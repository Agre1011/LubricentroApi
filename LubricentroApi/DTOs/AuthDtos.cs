using System.ComponentModel.DataAnnotations;

namespace LubricentroApi.DTOs
{
    // Datos que se envían para iniciar sesión.
    public class LoginDto
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }

    // Datos necesarios para crear un nuevo usuario.
    public class CrearUsuarioDto
    {
        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Apellido { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string Rol { get; set; } = "Empleado";
    }

    // Respuesta que devolverá el login.
    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;

        public string Username { get; set; } = string.Empty;

        public string Rol { get; set; } = string.Empty;

        public DateTime Expiracion { get; set; }
    }
}