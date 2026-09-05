using LubricentroApi.Data;
using LubricentroApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ----------------------------------------------------
// CONTROLADORES
// ----------------------------------------------------
builder.Services.AddControllers();

// ----------------------------------------------------
// OPENAPI
// ----------------------------------------------------
builder.Services.AddOpenApi();

// ----------------------------------------------------
// BASE DE DATOS
// ----------------------------------------------------
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

// ----------------------------------------------------
// CORS PERMISIVO
// ----------------------------------------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPermisivo", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// ----------------------------------------------------
// CONFIGURACIÓN JWT
// ----------------------------------------------------
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("No se encontró la clave JWT.");

var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme =
        JwtBearerDefaults.AuthenticationScheme;

    options.DefaultChallengeScheme =
        JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters =
        new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey)
            )
        };
});

builder.Services.AddAuthorization();
// ----------------------------------------------------
// SERVICIOS
// ----------------------------------------------------
builder.Services.AddScoped<JwtService>();

var app = builder.Build();

// ----------------------------------------------------
// PIPELINE HTTP
// ----------------------------------------------------
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// CORS antes de autenticación y autorización.
app.UseCors("CorsPermisivo");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();