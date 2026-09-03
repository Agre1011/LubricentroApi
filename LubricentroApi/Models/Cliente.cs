using System.ComponentModel.DataAnnotations;

namespace LubricentroApi.Models
{
    public class Cliente
    {
        [Key]
        public int IdCliente { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Apellido { get; set; }

        [MaxLength(20)]
        public string? CUIL { get; set; }

        [MaxLength(30)]
        public string? Telefono { get; set; }

        [MaxLength(150)]
        [EmailAddress]
        public string? Email { get; set; }

        public bool Activo { get; set; } = true;

        // Relación: un cliente puede tener muchas salidas o ventas.
        public ICollection<Salida> Salidas { get; set; } = new List<Salida>();
    }
}