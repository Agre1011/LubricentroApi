using System.ComponentModel.DataAnnotations;

namespace LubricentroApi.Models
{
    public class Usuario
    {
        [Key]
        public int IdUsuario { get; set; }

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
        [MaxLength(150)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(300)]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [MaxLength(30)]
        public string Rol { get; set; } = "Empleado";

        public bool Activo { get; set; } = true;

        // Relación: un usuario puede registrar muchos ingresos.
        public ICollection<Ingreso> Ingresos { get; set; } = new List<Ingreso>();

        // Relación: un usuario puede registrar muchas salidas o ventas.
        public ICollection<Salida> Salidas { get; set; } = new List<Salida>();
    }
}