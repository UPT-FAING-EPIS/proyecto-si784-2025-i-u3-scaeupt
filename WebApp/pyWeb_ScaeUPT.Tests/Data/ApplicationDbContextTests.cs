using Microsoft.EntityFrameworkCore;
using Xunit;
using pyWeb_ScaeUPT.Data;
using pyWeb_ScaeUPT.Models;

namespace pyWeb_ScaeUPT.Tests.Data
{
    public class ApplicationDbContextTests
    {
        [Fact]
        public void ApplicationDbContext_CanAddAndRetrieveEstudiante()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new ApplicationDbContext(options);
            var estudiante = new estudianteModel
            {
                Id_Persona = "12345678",
                Matricula = "2021001",
                Semestre = 8,
                Correo = "test@virtual.upt.pe"
            };

            // Act
            context.estudiante.Add(estudiante);
            context.SaveChanges();

            // Assert
            var retrievedEstudiante = context.estudiante.First();
            Assert.Equal("12345678", retrievedEstudiante.Id_Persona);
            Assert.Equal("2021001", retrievedEstudiante.Matricula);
        }

        [Fact]
        public void ApplicationDbContext_CanAddAndRetrieveToken()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new ApplicationDbContext(options);
            var token = new tokenModel
            {
                DNI_token = "12345678",
                Token = "encrypted-token"
            };

            // Act
            context.token.Add(token);
            context.SaveChanges();

            // Assert
            var retrievedToken = context.token.First();
            Assert.Equal("12345678", retrievedToken.DNI_token);
            Assert.Equal("encrypted-token", retrievedToken.Token);
        }
    }
}