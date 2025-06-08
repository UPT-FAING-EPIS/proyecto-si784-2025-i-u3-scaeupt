using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using pyWeb_ScaeUPT.Models;
using System;
using System.Security.Cryptography;
using System.Text;
using pyWeb_ScaeUPT.Data;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using QRCoder;
using System.IO;

namespace pyWeb_ScaeUPT.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly string _encryptionKey = "ScaeUPT2024SecretKey123456789012345";

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { message = "API funcionando correctamente" });
        }

        [HttpGet("estudiante/{id}")]
        public IActionResult GetEstudiante(int id)
        {
            var estudiante = _dbContext.estudiante.Find(id);
            if (estudiante == null) return NotFound();
            return Ok(estudiante);
        }

        [HttpGet("user-info")]
        [Authorize]
        public IActionResult GetUserInfo()
        {
            try
            {
                // Obtener el ID del usuario desde el token JWT
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int id))
                {
                    return BadRequest("ID de usuario no válido");
                }

                // Buscar el estudiante en la base de datos
                var estudiante = _dbContext.estudiante.Find(id);
                if (estudiante == null) return NotFound("Estudiante no encontrado");

                // Obtener el correo del token
                var email = User.FindFirst(ClaimTypes.Email)?.Value;

                // Crear un objeto con la información del usuario
                var userInfo = new
                {
                    id = estudiante.Id_Estudiante,
                    name = email.Split('@')[0], // Usar la parte del correo antes del @ como nombre
                    email = email,
                    matricula = estudiante.Matricula
                };

                return Ok(userInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener información del usuario");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpPost("generate-qr")]
        [Authorize]
        public IActionResult GenerateQR()
        {
            try
            {
                // Obtener el ID del usuario desde el token JWT
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int id))
                {
                    return BadRequest("ID de usuario no válido");
                }

                // Buscar el estudiante en la base de datos
                var estudiante = _dbContext.estudiante.Find(id);
                if (estudiante == null) return NotFound("Estudiante no encontrado");

                // Para este ejemplo, usaremos el DNI (Id_Persona) como dato a encriptar
                string dniToEncrypt = estudiante.Id_Persona;
                
                // Generar un token encriptado con el DNI y la fecha/hora actual
                string dataToEncrypt = $"{dniToEncrypt}|{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}|{estudiante.Matricula}";
                string encryptedToken = EncryptData(dataToEncrypt);

                // Crear o actualizar el registro en la tabla de tokens
                var existingToken = _dbContext.token.FirstOrDefault(t => t.DNI_token == dniToEncrypt);
                if (existingToken != null)
                {
                    // Actualizar token existente
                    existingToken.Token = encryptedToken;
                }
                else
                {
                    // Crear nuevo token
                    _dbContext.token.Add(new tokenModel
                    {
                        DNI_token = dniToEncrypt,
                        Token = encryptedToken
                    });
                }

                _dbContext.SaveChanges();

                // Generar el código QR
                string qrCodeBase64 = GenerateQRCodeBase64(encryptedToken);

                // Devolver la información necesaria para mostrar en el frontend
                return Ok(new { 
                    success = true, 
                    timestamp = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                    qrData = encryptedToken,
                    qrCodeBase64 = qrCodeBase64
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar código QR");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        // Método para encriptar datos usando AES
        private string EncryptData(string data)
        {
            try
            {
                byte[] encryptedBytes;
                using (Aes aes = Aes.Create())
                {
                    // Derivar clave y vector de inicialización (IV) usando PBKDF2
                    using (var deriveBytes = new Rfc2898DeriveBytes(_encryptionKey, new byte[16], 1000))
                    {
                        aes.Key = deriveBytes.GetBytes(32); // 256 bits
                        aes.IV = deriveBytes.GetBytes(16);  // 128 bits
                    }

                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;

                    // Crear encriptador
                    ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                    // Encriptar los datos
                    using (var ms = new MemoryStream())
                    {
                        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                        {
                            byte[] dataBytes = Encoding.UTF8.GetBytes(data);
                            cs.Write(dataBytes, 0, dataBytes.Length);
                        }
                        encryptedBytes = ms.ToArray();
                    }
                }

                // Convertir a Base64 para facilitar el almacenamiento
                return Convert.ToBase64String(encryptedBytes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al encriptar datos");
                throw;
            }
        }

        // Método para desencriptar datos usando AES
        private string DecryptData(string encryptedData)
        {
            try
            {
                byte[] encryptedBytes = Convert.FromBase64String(encryptedData);
                string decryptedData;

                using (Aes aes = Aes.Create())
                {
                    // Derivar clave y vector de inicialización (IV) usando PBKDF2
                    using (var deriveBytes = new Rfc2898DeriveBytes(_encryptionKey, new byte[16], 1000))
                    {
                        aes.Key = deriveBytes.GetBytes(32); // 256 bits
                        aes.IV = deriveBytes.GetBytes(16);  // 128 bits
                    }

                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;

                    // Crear desencriptador
                    ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                    // Desencriptar los datos
                    using (var ms = new MemoryStream(encryptedBytes))
                    {
                        using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                        {
                            using (var sr = new StreamReader(cs))
                            {
                                decryptedData = sr.ReadToEnd();
                            }
                        }
                    }
                }

                return decryptedData;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al desencriptar datos");
                throw;
            }
        }

        // Método para generar un código QR como imagen Base64
        private string GenerateQRCodeBase64(string data)
        {
            try
            {
                using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
                {
                    QRCodeData qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
                    using (PngByteQRCode qrCode = new PngByteQRCode(qrCodeData))
                    {
                        byte[] qrCodeImage = qrCode.GetGraphic(20);
                        return Convert.ToBase64String(qrCodeImage);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar código QR como Base64");
                throw;
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [HttpGet("error")]
        public IActionResult Error()
        {
            return StatusCode(500, new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
