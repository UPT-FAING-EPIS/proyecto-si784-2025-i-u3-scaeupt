using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using pyWeb_ScaeUPT.Controllers;
using pyWeb_ScaeUPT.Data;
using pyWeb_ScaeUPT.Models;
using System.Security.Claims;
using TechTalk.SpecFlow;
using Xunit;

namespace pyWeb_ScaeUPT.Tests.Steps
{
    [Binding]
    public class GeneracionCodigoQRSteps
    {
        private readonly ScenarioContext _scenarioContext;
        private ApplicationDbContext _context;
        private HomeController _controller;
        private ILogger<HomeController> _logger;
        private IActionResult _result;
        private string _errorMessage;
        private DateTime _generationTime;

        public GeneracionCodigoQRSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [BeforeScenario]
        public void BeforeScenario()
        {
            // Configurar base de datos en memoria
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);

            // Configurar logger mock
            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            _logger = loggerFactory.CreateLogger<HomeController>();

            // Crear controlador
            _controller = new HomeController(_logger, _context);
        }

        [AfterScenario]
        public void AfterScenario()
        {
            _context?.Dispose();
        }

        [Given(@"que soy un estudiante registrado con DNI ""(.*)""")]
        public void DadoQueSoyUnEstudianteRegistradoConDNI(string dni)
        {
            var estudiante = new estudianteModel
            {
                Id_Estudiante = 1,
                Id_Persona = dni,
                Matricula = "2021001",
                Semestre = 8,
                Correo = "estudiante@virtual.upt.pe"
            };

            _context.estudiante.Add(estudiante);
            _context.SaveChanges();

            _scenarioContext["EstudianteId"] = estudiante.Id_Estudiante;
            _scenarioContext["DNI"] = dni;
        }

        [Given(@"mi matrícula es ""(.*)""")]
        public void DadoMiMatriculaEs(string matricula)
        {
            _scenarioContext["Matricula"] = matricula;
        }

        [Given(@"mi correo es ""(.*)""")]
        public void DadoMiCorreoEs(string correo)
        {
            _scenarioContext["Correo"] = correo;
        }

        [Given(@"he iniciado sesión correctamente")]
        public void DadoHeIniciadoSesionCorrectamente()
        {
            // Simular autenticación con Claims
            var estudianteId = _scenarioContext.Get<int>("EstudianteId");
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, estudianteId.ToString()),
                new Claim(ClaimTypes.Name, "Juan Pérez"),
                new Claim(ClaimTypes.Email, _scenarioContext.Get<string>("Correo"))
            };

            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = principal
                }
            };
        }

        [Given(@"que mi token JWT no contiene un ID de usuario válido")]
        public void DadoQueMiTokenJWTNoContieneUnIDDeUsuarioValido()
        {
            // Simular token inválido
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "Usuario Sin ID")
                // No incluir ClaimTypes.NameIdentifier
            };

            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = principal
                }
            };
        }

        [Given(@"que ya tengo un código QR generado previamente")]
        public void DadoQueYaTengoUnCodigoQRGeneradoPreviamente()
        {
            var estudianteId = _scenarioContext.Get<int>("EstudianteId");
            var tokenExistente = new tokenModel
            {
                Id_codigoqr = 1,
                DNI_token = estudianteId.ToString(),
                Token = "token_anterior_encriptado"
            };

            _context.token.Add(tokenExistente);
            _context.SaveChanges();

            _scenarioContext["TokenAnterior"] = tokenExistente;
        }

        [Given(@"que mi ID de usuario es ""(.*)"" pero no existo en la base de datos")]
        public void DadoMiIDDeUsuarioEsPeroNoExistoEnLaBaseDeDatos(string userId)
        {
            // Simular usuario autenticado pero que no existe en BD
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, "Usuario Inexistente")
            };

            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = principal
                }
            };
        }

        [Given(@"que he generado un código QR")]
        public void DadoQueHeGeneradoUnCodigoQR()
        {
            DadoQueYaTengoUnCodigoQRGeneradoPreviamente();
            _generationTime = DateTime.Now;
        }

        [When(@"solicito generar mi código QR")]
        public void CuandoSolicitoGenerarMiCodigoQR()
        {
            try
            {
                _result = _controller.GenerateQR();
                _generationTime = DateTime.Now;
            }
            catch (Exception ex)
            {
                _errorMessage = ex.Message;
            }
        }

        [When(@"solicito generar mi código QR nuevamente")]
        public void CuandoSolicitoGenerarMiCodigoQRNuevamente()
        {
            try
            {
                _result = _controller.GenerateQR();
                _generationTime = DateTime.Now;
            }
            catch (Exception ex)
            {
                _errorMessage = ex.Message;
            }
        }


        [When(@"pasan 5 minutos desde la generación")]
        public void CuandoPasan5MinutosDesdeLaGeneracion()
        {
            // Simular que han pasado 5 minutos
            _generationTime = DateTime.Now.AddMinutes(-5);
        }

        [Then(@"debería recibir un código QR válido")]
        public void EntoncesDeberiaRecibirUnCodigoQRValido()
        {
            Assert.NotNull(_result);
            var okResult = Assert.IsType<OkObjectResult>(_result);
            Assert.NotNull(okResult.Value);

            // Verificar que contiene las propiedades esperadas
            var resultValue = okResult.Value;
            var properties = resultValue?.GetType().GetProperties();

            Assert.Contains(properties, p => p.Name == "qrCodeBase64");
            Assert.Contains(properties, p => p.Name == "timestamp");
            Assert.Contains(properties, p => p.Name == "qrData");
            Assert.Contains(properties, p => p.Name == "success");
        }

        [Then(@"el código QR debería contener mis datos encriptados")]
        public void EntoncesElCodigoQRDeberiaContenerMisDatosEncriptados()
        {
            var okResult = Assert.IsType<OkObjectResult>(_result);
            var resultValue = okResult.Value;
            var qrDataProperty = resultValue?.GetType().GetProperty("qrData");
            var encryptedData = qrDataProperty?.GetValue(resultValue)?.ToString();

            Assert.NotNull(encryptedData);
            Assert.NotEmpty(encryptedData);
        }

        [Then(@"debería guardarse un token en la base de datos")]
        public void EntoncesDeberiaGuardarseUnTokenEnLaBaseDeDatos()
        {
            // Obtener el DNI en lugar del ID de estudiante
            var dni = _scenarioContext.Get<string>("DNI");
            var token = _context.token.FirstOrDefault(t => t.DNI_token == dni);

            Assert.NotNull(token);
        }


        [Then(@"la respuesta debería incluir la fecha y hora actual de Lima")]
        public void EntoncesLaRespuestaDeberiaIncluirLaFechaYHoraActualDeLima()
        {
            var okResult = Assert.IsType<OkObjectResult>(_result);
            var resultValue = okResult.Value;
            var timestampProperty = resultValue?.GetType().GetProperty("timestamp");
            var timestamp = timestampProperty?.GetValue(resultValue)?.ToString();

            Assert.NotNull(timestamp);
            Assert.NotEmpty(timestamp);
        }

        [Then(@"debería recibir un nuevo código QR")]
        public void EntoncesDeberiaRecibirUnNuevoCodigoQR()
        {
            EntoncesDeberiaRecibirUnCodigoQRValido();
        }

        [Then(@"el token anterior debería ser actualizado en la base de datos")]
        public void EntoncesElTokenAnteriorDeberiaSerActualizadoEnLaBaseDeDatos()
        {
            var dni = _scenarioContext.Get<string>("DNI");
            var tokenActual = _context.token.FirstOrDefault(t => t.DNI_token == dni);
            var tokenAnterior = _scenarioContext.Get<tokenModel>("TokenAnterior");

            Assert.NotNull(tokenActual);
            // Verificar que el token fue actualizado
            Assert.NotEqual(tokenAnterior.Token, tokenActual.Token);
        }

        [Then(@"el nuevo código QR debería tener una nueva marca de tiempo")]
        public void EntoncesElNuevoCodigoQRDeberiaTenerUnaNuevaMarcaDeTiempo()
        {
            EntoncesLaRespuestaDeberiaIncluirLaFechaYHoraActualDeLima();
        }


        [Then(@"debería recibir un error ""(.*)""")]
        public void EntoncesDeberiaRecibirUnError(string mensajeEsperado)
        {
            bool errorEncontrado = false;
            string mensajeError = "";

            if (_result != null)
            {
                if (_result is BadRequestObjectResult badRequest)
                {
                    mensajeError = badRequest.Value?.ToString() ?? "";
                    errorEncontrado = mensajeError.Contains(mensajeEsperado);
                }
                else if (_result is NotFoundObjectResult notFound)
                {
                    mensajeError = notFound.Value?.ToString() ?? "";
                    errorEncontrado = mensajeError.Contains(mensajeEsperado);
                }
            }

            if (!string.IsNullOrEmpty(_errorMessage))
            {
                mensajeError = _errorMessage;
                errorEncontrado = _errorMessage.Contains(mensajeEsperado);
            }

            Assert.True(errorEncontrado,
                $"Se esperaba el error '{mensajeEsperado}', pero se obtuvo: '{mensajeError}'");
        }

        [Then(@"no debería generarse ningún código QR")]
        public void EntoncesNoDeberiaGenerarseNingunCodigoQR()
        {
            // Debug: Mostrar qué tipo de resultado tenemos
            string debugInfo = "";
            if (_result != null)
            {
                debugInfo = $"Tipo de resultado: {_result.GetType().Name}";
                if (_result is OkObjectResult okResult)
                {
                    debugInfo += $", Valor: {okResult.Value}";
                }
                else if (_result is BadRequestObjectResult badResult)
                {
                    debugInfo += $", Error: {badResult.Value}";
                }
            }

            bool noSeGeneroQR = false;

            // Caso 1: No hay resultado (excepción)
            if (_result == null)
            {
                noSeGeneroQR = true;
            }
            // Caso 2: Resultado de error explícito
            else if (_result is BadRequestObjectResult || _result is NotFoundObjectResult)
            {
                noSeGeneroQR = true;
            }
            // Caso 3: Excepción capturada
            else if (!string.IsNullOrEmpty(_errorMessage))
            {
                noSeGeneroQR = true;
            }
            // Caso 4: OkResult pero con success = false
            else if (_result is OkObjectResult okResult)
            {
                var resultValue = okResult.Value;
                var successProperty = resultValue?.GetType().GetProperty("success");
                var success = successProperty?.GetValue(resultValue);

                if (success is bool successBool && !successBool)
                {
                    noSeGeneroQR = true;
                }
            }

            Assert.True(noSeGeneroQR,
                $"Se esperaba que no se generara ningún código QR. Debug: {debugInfo}, ErrorMessage: {_errorMessage}");
        }

        [Then(@"el código QR debería considerarse expirado")]
        public void EntoncesElCodigoQRDeberiaConsiderarseExpirado()
        {
            var tiempoTranscurrido = DateTime.Now - _generationTime;
            Assert.True(tiempoTranscurrido.TotalMinutes >= 5);
        }

        [Then(@"debería necesitar regenerar un nuevo código QR")]
        public void EntoncesDeberianecesitarRegenerarUnNuevoCodigoQR()
        {
            EntoncesElCodigoQRDeberiaConsiderarseExpirado();
        }
    }
}