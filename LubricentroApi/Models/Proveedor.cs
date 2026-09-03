using System.ComponentModel.DataAnnotations;

namespace LubricentroApi.Models
{
    public class Proveedor
    {
        [Key]
        public int IdProveedor { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string CUIT { get; set; } = string.Empty;

        [MaxLength(30)]
        public string? Telefono { get; set; }

        [MaxLength(150)]
        [EmailAddress]
        public string? Email { get; set; }

        public bool Activo { get; set; } = true;

        // Relación: un proveedor puede tener muchos ingresos.
        public ICollection<Ingreso> Ingresos { get; set; } = new List<Ingreso>();
    }
}