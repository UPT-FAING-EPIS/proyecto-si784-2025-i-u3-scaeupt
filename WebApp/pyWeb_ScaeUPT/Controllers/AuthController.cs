using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using System;
using System.Text.Json;
using pyWeb_ScaeUPT.Models;
using pyWeb_ScaeUPT.Data;

using Microsoft.AspNetCore.Authorization;

namespace pyWeb_ScaeUPT.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<AuthController> _logger;
        private readonly HttpClient _httpClient;

        public AuthController(IConfiguration configuration, ApplicationDbContext dbContext, 
            ILogger<AuthController> logger, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _dbContext = dbContext;
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient();
        }

        [HttpPost("google")]
        public async Task<IActionResult> Google([FromBody] GoogleAuthRequest request)
        {
            try
            {
                _logger.LogInformation($"Received Google ID token: {request.IdToken}");
                
                // Verificar el token de Google
                var payload = await VerifyGoogleToken(request.IdToken);
                if (payload == null)
                {
                    return BadRequest(new { error = "Token de Google inválido" });
                }

                // Verificar si el correo pertenece al dominio institucional
                if (!payload.Email.EndsWith("@virtual.upt.pe"))
                {
                    return BadRequest(new { error = "Solo se permite el acceso con cuentas del dominio virtual.upt.pe" });
                }

                // Buscar o crear el usuario en la base de datos
                var usuario = await BuscarEstudiante(payload);

                if (usuario == null)
                {
                    return BadRequest("El correo no está registrado en el sistema");
                }

                // Generar token JWT
                var token = GenerarJWT(usuario);

                return Ok(new
                {
                    token = token,
                    user = new
                    {
                        id = usuario.Id_Estudiante,
                        name = payload.Name,
                        email = payload.Email,
                        picture = payload.Picture
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en la autenticación con Google");
                return StatusCode(500, new { error = "Error en la autenticación", message = ex.Message });
            }
        }

        [HttpGet("validate")]
        [Authorize]
        public IActionResult ValidateToken()
        {
            // Este endpoint simplemente valida que el token JWT sea válido
            // La validación se hace automáticamente por el middleware de autenticación
            return Ok(new { valid = true });
        }

        private async Task<GoogleTokenPayload> VerifyGoogleToken(string idToken)
        {
            try
            {
                // Verificar el token con la API de Google
                var response = await _httpClient.GetAsync(
                    $"https://oauth2.googleapis.com/tokeninfo?id_token={idToken}");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"Google token verification failed with status: {response.StatusCode}");
                    return null;
                }

                var content = await response.Content.ReadAsStringAsync();
                //_logger.LogInformation($"Google token response: {content}");
                
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                
                var payload = JsonSerializer.Deserialize<GoogleTokenPayload>(content, options);

                // Verificar token fue emitido
                if (payload.Aud != _configuration["Authentication:Google:ClientId"])
                {
                    _logger.LogWarning($"Token audience mismatch. Expected: {_configuration["Authentication:Google:ClientId"]}, Got: {payload.Aud}");
                    return null;
                }

                return payload;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying Google token");
                return null;
            }
        }

        private async Task<estudianteModel> BuscarEstudiante(GoogleTokenPayload payload)
        {
            // Buscar estudiante por correo
            var estudiante = _dbContext.estudiante
                .FirstOrDefault(e => e.Correo == payload.Email);

            return estudiante;
        }

        private string GenerarJWT(estudianteModel usuario)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration["JWT:SecretKey"] ?? Environment.GetEnvironmentVariable("JWT_SECRET_KEY")));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, usuario.Id_Estudiante.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, usuario.Correo),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"] ?? "pyWeb_ScaeUPT",
                audience: _configuration["JWT:Audience"] ?? "pyWeb_ScaeUPT_Clients",
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class GoogleAuthRequest
    {
        public string IdToken { get; set; }
    }

    public class GoogleTokenPayload
    {
        public string Iss { get; set; }
        public string Azp { get; set; }
        public string Aud { get; set; }
        public string Sub { get; set; }
        public string Email { get; set; }
        public bool EmailVerified { get; set; }
        public string Name { get; set; }
        public string Picture { get; set; }
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public string Iat { get; set; }
        public string Exp { get; set; }
    }
}