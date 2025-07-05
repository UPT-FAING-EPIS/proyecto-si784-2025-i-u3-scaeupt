using Microsoft.EntityFrameworkCore;
using pyWeb_ScaeUPT.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using pyWeb_ScaeUPT.Services;
using DotNetEnv; // AGREGAR ESTA LÍNEA

var builder = WebApplication.CreateBuilder(args);

// AGREGAR: Cargar variables de entorno
Env.Load();

// AGREGAR: Configurar variables de entorno en builder
builder.Configuration["Authentication:Google:ClientId"] = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID");
builder.Configuration["Authentication:Google:ClientSecret"] = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_SECRET");
builder.Configuration["JWT:SecretKey"] = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Application Insights
builder.Services.AddApplicationInsightsTelemetry(options =>
{
    options.ConnectionString = builder.Configuration.GetConnectionString("ApplicationInsights") 
        ?? Environment.GetEnvironmentVariable("APPLICATIONINSIGHTS_CONNECTION_STRING");
});

// Registrar TelemetryClient como singleton
builder.Services.AddSingleton<TelemetryClient>();

// Registrar MetricsService
builder.Services.AddScoped<IMetricsService, MetricsService>();

// CORS - MOVER ANTES DE OTROS SERVICIOS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});

// HttpClient
builder.Services.AddHttpClient();

// Database context
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? Environment.GetEnvironmentVariable("MYSQL_CONNECTION_STRING");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// JWT Authentication - CORREGIR CONFIGURACIÓN
var jwtKey = builder.Configuration["JWT:SecretKey"] ?? Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
var issuer = builder.Configuration["JWT:Issuer"];
var audience = builder.Configuration["JWT:Audience"];
var key = Encoding.UTF8.GetBytes(jwtKey); // CAMBIAR A UTF8

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,          // CAMBIAR A true
        ValidateAudience = true,        // CAMBIAR A true
        ValidateLifetime = true,        // AGREGAR
        ValidateIssuerSigningKey = true,
        ValidIssuer = issuer,           // AGREGAR
        ValidAudience = audience,       // AGREGAR
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

// AGREGAR: Autorización explícita
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/api/Home/error"); // AGREGAR manejo de errores
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("AllowAll");

app.UseRouting(); // AGREGAR ESTA LÍNEA

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();

// Program accesible para las pruebas
public partial class Program { }
