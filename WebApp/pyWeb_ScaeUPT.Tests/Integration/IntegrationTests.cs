using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using Xunit;
using pyWeb_ScaeUPT.Data;
using pyWeb_ScaeUPT.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Hosting;

namespace pyWeb_ScaeUPT.Tests.Integration
{
    public class IntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public IntegrationTests(WebApplicationFactory<Program> factory)
        {
            // Configurar variables de entorno ANTES de crear la factory
            Environment.SetEnvironmentVariable("GOOGLE_CLIENT_ID", "test-client-id");
            Environment.SetEnvironmentVariable("GOOGLE_CLIENT_SECRET", "test-client-secret");
            Environment.SetEnvironmentVariable("JWT_SECRET_KEY", "test-secret-key-that-is-long-enough-for-jwt-256-bit");
            Environment.SetEnvironmentVariable("MYSQL_CONNECTION_STRING", "Server=localhost;Database=TestDb;Uid=test;Pwd=test;");

            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Testing");
                
                builder.ConfigureAppConfiguration((context, config) =>
                {
                    // Configurar todas las variables necesarias para las pruebas
                    config.AddInMemoryCollection(new Dictionary<string, string?>
                    {
                        ["JWT:SecretKey"] = "test-secret-key-that-is-long-enough-for-jwt-256-bit",
                        ["JWT:Issuer"] = "pyWeb_ScaeUPT",
                        ["JWT:Audience"] = "pyWeb_ScaeUPT_Clients",
                        ["Authentication:Google:ClientId"] = "test-client-id",
                        ["Authentication:Google:ClientSecret"] = "test-client-secret",
                        ["ConnectionStrings:MySqlConnection"] = "Server=localhost;Database=TestDb;Uid=test;Pwd=test;"
                    });
                });
                
                builder.ConfigureServices(services =>
                {
                    // Remover la configuración de DbContext existente
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                    if (descriptor != null)
                        services.Remove(descriptor);

                    // Agregar ApplicationDbContext usando una base de datos en memoria para pruebas
                    services.AddDbContext<ApplicationDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("InMemoryDbForTesting");
                    });

                    // Construir el proveedor de servicios
                    var sp = services.BuildServiceProvider();

                    // Crear un scope para obtener una referencia al contexto de la base de datos
                    using var scope = sp.CreateScope();
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<ApplicationDbContext>();

                    // Asegurar que la base de datos se cree
                    db.Database.EnsureCreated();

                    // Sembrar la base de datos con datos de prueba
                    SeedDatabase(db);
                });
            });

            _client = _factory.CreateClient();
        }

        private static void SeedDatabase(ApplicationDbContext context)
        {
            // Limpiar datos existentes
            context.estudiante.RemoveRange(context.estudiante);
            context.token.RemoveRange(context.token);
            context.SaveChanges();

            context.estudiante.Add(new estudianteModel
            {
                Id_Estudiante = 1,
                Id_Persona = "12345678",
                Matricula = "2021001",
                Semestre = 8,
                Correo = "test@virtual.upt.pe"
            });

            context.token.Add(new tokenModel
            {
                Id_codigoqr = 1,
                DNI_token = "12345678",
                Token = "sample-token"
            });

            context.SaveChanges();
        }

        [Fact]
        public async Task Get_HomeEndpoint_ReturnsSuccessAndCorrectContentType()
        {
            var response = await _client.GetAsync("/api/home");

            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine("STATUS: " + response.StatusCode);
            Console.WriteLine("BODY: " + content);

            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine("❌ EXCEPTION: " + ex.Message);
                Console.WriteLine("❌ RESPONSE CONTENT: " + content);
                throw;
            }

            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());
        }

        [Fact]
        public async Task Get_EstudianteEndpoint_ReturnsSuccessForValidId()
        {
            // Act
            var response = await _client.GetAsync("/api/home/estudiante/1");

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("12345678", content);
        }

        [Fact]
        public async Task Get_EstudianteEndpoint_ReturnsNotFoundForInvalidId()
        {
            // Act
            var response = await _client.GetAsync("/api/home/estudiante/999");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Post_AuthGoogle_ReturnsBadRequestForInvalidToken()
        {
            // Arrange
            var request = new { IdToken = "invalid-token" };
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/auth/google", content);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Get_ValidateToken_ReturnsUnauthorizedWithoutToken()
        {
            // Act
            var response = await _client.GetAsync("/api/auth/validate");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}