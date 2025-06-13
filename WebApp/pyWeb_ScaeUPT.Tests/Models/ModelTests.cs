using System.ComponentModel.DataAnnotations;
using Xunit;
using pyWeb_ScaeUPT.Models;

namespace pyWeb_ScaeUPT.Tests.Models
{
    public class ModelTests
    {
        [Fact]
        public void EstudianteModel_ValidData_PassesValidation()
        {
            // Arrange
            var estudiante = new estudianteModel
            {
                Id_Estudiante = 1,
                Id_Persona = "12345678",
                Matricula = "2021001",
                Semestre = 8,
                Correo = "test@virtual.upt.pe"
            };

            // Act
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(estudiante, new ValidationContext(estudiante), validationResults, true);

            // Assert
            Assert.True(isValid);
            Assert.Empty(validationResults);
        }

        [Fact]
        public void TokenModel_ValidData_PassesValidation()
        {
            // Arrange
            var token = new tokenModel
            {
                Id_codigoqr = 1,
                DNI_token = "12345678",
                Token = "encrypted-token-data"
            };

            // Act
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(token, new ValidationContext(token), validationResults, true);

            // Assert
            Assert.True(isValid);
            Assert.Empty(validationResults);
        }

        [Fact]
        public void EstudianteModel_Properties_SetCorrectly()
        {
            // Arrange & Act
            var estudiante = new estudianteModel
            {
                Id_Estudiante = 123,
                Id_Persona = "87654321",
                Matricula = "2024001",
                Semestre = 5,
                Correo = "estudiante@virtual.upt.pe"
            };

            // Assert
            Assert.Equal(123, estudiante.Id_Estudiante);
            Assert.Equal("87654321", estudiante.Id_Persona);
            Assert.Equal("2024001", estudiante.Matricula);
            Assert.Equal(5, estudiante.Semestre);
            Assert.Equal("estudiante@virtual.upt.pe", estudiante.Correo);
        }

        [Fact]
        public void TokenModel_Properties_SetCorrectly()
        {
            // Arrange & Act
            var token = new tokenModel
            {
                Id_codigoqr = 456,
                DNI_token = "11223344",
                Token = "sample-encrypted-token"
            };

            // Assert
            Assert.Equal(456, token.Id_codigoqr);
            Assert.Equal("11223344", token.DNI_token);
            Assert.Equal("sample-encrypted-token", token.Token);
        }
    }
}