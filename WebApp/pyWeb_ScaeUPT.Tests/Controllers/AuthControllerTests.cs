using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.Protected;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using pyWeb_ScaeUPT.Controllers;
using pyWeb_ScaeUPT.Data;
using pyWeb_ScaeUPT.Models;
using pyWeb_ScaeUPT.Services;

namespace pyWeb_ScaeUPT.Tests.Controllers
{
    public class AuthControllerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<ILogger<AuthController>> _mockLogger;
        private readonly Mock<IHttpClientFactory> _mockHttpClientFactory;
        private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            // Setup InMemory Database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);

            // Setup mocks
            _mockConfiguration = new Mock<IConfiguration>();
            _mockLogger = new Mock<ILogger<AuthController>>();
            _mockHttpClientFactory = new Mock<IHttpClientFactory>();
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();

            // Setup HttpClient mock - CORREGIDO: usar string.Empty en lugar del método de extensión
            var httpClient = new HttpClient(_mockHttpMessageHandler.Object);
            _mockHttpClientFactory.Setup(x => x.CreateClient(string.Empty))
                .Returns(httpClient);

            // Setup configuration
            _mockConfiguration.Setup(x => x["JWT:SecretKey"])
                .Returns("test-secret-key-that-is-long-enough-for-jwt-256-bit");
            _mockConfiguration.Setup(x => x["JWT:Issuer"])
                .Returns("pyWeb_ScaeUPT");
            _mockConfiguration.Setup(x => x["JWT:Audience"])
                .Returns("pyWeb_ScaeUPT_Clients");
            _mockConfiguration.Setup(x => x["Authentication:Google:ClientId"])
                .Returns("test-client-id");
            var mockMetricsService = new Mock<IMetricsService>();
            _controller = new AuthController(
                _mockConfiguration.Object,
                _context,
                _mockLogger.Object,
                _mockHttpClientFactory.Object,
                mockMetricsService.Object
            );

            // Seed test data
            SeedTestData();
        }

        private void SeedTestData()
        {
            var estudiante = new estudianteModel
            {
                Id_Estudiante = 1,
                Id_Persona = "12345678",
                Matricula = "2021001",
                Semestre = 8,
                Correo = "test@virtual.upt.pe"
            };
            _context.estudiante.Add(estudiante);
            _context.SaveChanges();
        }

        [Fact]
        public async Task Google_ValidToken_ReturnsOkWithJWT()
        {
            // Arrange
            var request = new pyWeb_ScaeUPT.Controllers.GoogleAuthRequest { IdToken = "valid-token" };
            var googleResponse = new
            {
                iss = "https://accounts.google.com",
                azp = "test-client-id",
                aud = "test-client-id",
                sub = "123456789",
                email = "test@virtual.upt.pe",
                email_verified = true,
                name = "Test User",
                picture = "https://example.com/picture.jpg",
                given_name = "Test",
                family_name = "User",
                iat = "1234567890",
                exp = "1234567890"
            };

            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(googleResponse))
                });

            // Act
            var result = await _controller.Google(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task Google_InvalidDomain_ReturnsBadRequest()
        {
            // Arrange
            var request = new pyWeb_ScaeUPT.Controllers.GoogleAuthRequest { IdToken = "valid-token" };
            var googleResponse = new
            {
                iss = "https://accounts.google.com",
                azp = "test-client-id",
                aud = "test-client-id",
                sub = "123456789",
                email = "test@gmail.com", // Invalid domain
                email_verified = true,
                name = "Test User",
                picture = "https://example.com/picture.jpg",
                given_name = "Test",
                family_name = "User",
                iat = "1234567890",
                exp = "1234567890"
            };

            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(googleResponse))
                });

            // Act
            var result = await _controller.Google(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var resultValue = badRequestResult.Value?.ToString();
            Assert.NotNull(resultValue);
            Assert.Contains("virtual.upt.pe", resultValue);
        }

        [Fact]
        public async Task Google_InvalidToken_ReturnsBadRequest()
        {
            // Arrange
            var request = new pyWeb_ScaeUPT.Controllers.GoogleAuthRequest { IdToken = "invalid-token" };

            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest
                });

            // Act
            var result = await _controller.Google(request);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void ValidateToken_ReturnsOk()
        {
            // Act
            var result = _controller.ValidateToken();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}