using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LubricentroApi.Models
{
    public class Producto
    {
        [Key]
        public int IdProducto { get; set; }

        [Required]
        [MaxLength(150)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Marca { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Variante { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecioCompra { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecioVenta { get; set; }

        public int Stock { get; set; }

        [MaxLength(300)]
        public string? Imagen { get; set; }

        public int IdCategoria { get; set; }

        public bool Activo { get; set; } = true;

        // Relación: cada producto pertenece a una categoría.
        [ForeignKey(nameof(IdCategoria))]
        public Categoria? Categoria { get; set; }

        // Relación: un producto puede aparecer en muchos detalles de ingreso.
        public ICollection<DetalleIngreso> DetallesIngresos { get; set; }
            = new List<DetalleIngreso>();

        // Relación: un producto puede aparecer en muchos detalles de salida.
        public ICollection<DetalleSalida> DetallesSalidas { get; set; }
            = new List<DetalleSalida>();
    }
}