using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using Xunit;
using pyWeb_ScaeUPT.Controllers;
using pyWeb_ScaeUPT.Data;
using pyWeb_ScaeUPT.Models;

namespace pyWeb_ScaeUPT.Tests.Controllers
{
    public class HomeControllerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly Mock<ILogger<HomeController>> _mockLogger;
        private readonly HomeController _controller;

        public HomeControllerTests()
        {
            // Setup InMemory Database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);

            _mockLogger = new Mock<ILogger<HomeController>>();
            _controller = new HomeController(_mockLogger.Object, _context);

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
        public void Get_ReturnsOkWithMessage()
        {
            // Act
            var result = _controller.Get();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public void GetEstudiante_ExistingId_ReturnsOk()
        {
            // Act
            var result = _controller.GetEstudiante(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var estudiante = Assert.IsType<estudianteModel>(okResult.Value);
            Assert.Equal(1, estudiante.Id_Estudiante);
        }

        [Fact]
        public void GetEstudiante_NonExistingId_ReturnsNotFound()
        {
            // Act
            var result = _controller.GetEstudiante(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void GetUserInfo_ValidUser_ReturnsOk()
        {
            // Arrange
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Email, "test@virtual.upt.pe")
            };
            var identity = new ClaimsIdentity(claims, "Test");
            var principal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };

            // Act
            var result = _controller.GetUserInfo();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public void GetUserInfo_InvalidUserId_ReturnsBadRequest()
        {
            // Arrange
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "invalid"),
                new Claim(ClaimTypes.Email, "test@virtual.upt.pe")
            };
            var identity = new ClaimsIdentity(claims, "Test");
            var principal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };

            // Act
            var result = _controller.GetUserInfo();

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void GenerateQR_ValidUser_ReturnsOk()
        {
            // Arrange
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Email, "test@virtual.upt.pe")
            };
            var identity = new ClaimsIdentity(claims, "Test");
            var principal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };

            // Act
            var result = _controller.GenerateQR();

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