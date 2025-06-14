using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

Env.Load();
//google
builder.Configuration["Authentication:Google:ClientId"] = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID");
builder.Configuration["Authentication:Google:ClientSecret"] = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_SECRET");
//jwt
builder.Configuration["JWT:SecretKey"] = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");

// Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

builder.Services.AddHttpClient();

builder.Services.AddControllers();

// Configurar Entity Framework Core con MySQL
// builder.Services.AddDbContext<pyWeb_ScaeUPT.Data.ApplicationDbContext>(options =>
//     options.UseMySql(
//         builder.Configuration.GetConnectionString("MySqlConnection"),
//         ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("MySqlConnection"))
//     ));


builder.Services.AddDbContext<pyWeb_ScaeUPT.Data.ApplicationDbContext>(options =>
    options.UseMySql(
        Environment.GetEnvironmentVariable("MYSQL_CONNECTION_STRING"),
        ServerVersion.AutoDetect(Environment.GetEnvironmentVariable("MYSQL_CONNECTION_STRING"))
    ));


// Leer valores de configuración para JWT
var secretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
var issuer = builder.Configuration["JWT:Issuer"];
var audience = builder.Configuration["JWT:Audience"];

// ✅ Configurar autenticación JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});

// Agregar autorización
builder.Services.AddAuthorization();

var app = builder.Build();

// Middleware pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/api/Home/error");
    app.UseHsts();

}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("AllowAll");

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapFallbackToFile("index.html"); //para malass reedirecciones

app.Run();

// Program accesible para las pruebas
public partial class Program { }
