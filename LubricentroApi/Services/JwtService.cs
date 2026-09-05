using LubricentroApi.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LubricentroApi.Services
{
    public class JwtService
    {
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public (string Token, DateTime Expiracion) GenerarToken(Usuario usuario)
        {
            // Obtenemos la configuración JWT desde appsettings.json.
            string jwtKey = _configuration["Jwt:Key"]
                ?? throw new InvalidOperationException("No se encontró Jwt:Key.");

            string jwtIssuer = _configuration["Jwt:Issuer"]
                ?? throw new InvalidOperationException("No se encontró Jwt:Issuer.");

            string jwtAudience = _configuration["Jwt:Audience"]
                ?? throw new InvalidOperationException("No se encontró Jwt:Audience.");

            // Leemos el tiempo de expiración configurado.
            if (!int.TryParse(
                _configuration["Jwt:ExpireMinutes"],
                out int expireMinutes))
            {
                expireMinutes = 120;
            }

            DateTime expiracion =
                DateTime.UtcNow.AddMinutes(expireMinutes);

            // Información que viajará dentro del token.
            var claims = new List<Claim>
            {
                new Claim(
                    JwtRegisteredClaimNames.Sub,
                    usuario.IdUsuario.ToString()
                ),

                new Claim(
                    ClaimTypes.NameIdentifier,
                    usuario.IdUsuario.ToString()
                ),

                new Claim(
                    ClaimTypes.Name,
                    usuario.Username
                ),

                new Claim(
                    ClaimTypes.Email,
                    usuario.Email
                ),

                new Claim(
                    ClaimTypes.Role,
                    usuario.Rol
                ),

                new Claim(
                    JwtRegisteredClaimNames.Jti,
                    Guid.NewGuid().ToString()
                )
            };

            // Generamos la clave de firma.
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey)
            );

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256
            );

            // Creamos el JWT.
            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: expiracion,
                signingCredentials: credentials
            );

            string tokenString =
                new JwtSecurityTokenHandler()
                    .WriteToken(token);

            return (tokenString, expiracion);
        }
    }
}