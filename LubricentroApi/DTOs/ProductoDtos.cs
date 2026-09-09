using System.ComponentModel.DataAnnotations;

namespace LubricentroApi.DTOs
{
    // Datos necesarios para crear un producto.
    public class CrearProductoDto
    {
        [Required]
        [MaxLength(150)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Marca { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Variante { get; set; } = string.Empty;

        [Range(0.01, double.MaxValue)]
        public decimal PrecioCompra { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal PrecioVenta { get; set; }

        [Range(1, int.MaxValue)]
        public int IdCategoria { get; set; }
    }

    // Datos que permitimos modificar de un producto.
    public class EditarProductoDto
    {
        [Required]
        [MaxLength(150)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Marca { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Variante { get; set; } = string.Empty;

        [Range(0.01, double.MaxValue)]
        public decimal PrecioCompra { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal PrecioVenta { get; set; }

        [Range(1, int.MaxValue)]
        public int IdCategoria { get; set; }
    }

    // Información que devolverá la API al consultar productos.
    public class ProductoResponseDto
    {
        public int IdProducto { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public string Marca { get; set; } = string.Empty;

        public string Variante { get; set; } = string.Empty;

        public decimal PrecioCompra { get; set; }

        public decimal PrecioVenta { get; set; }

        public int Stock { get; set; }

        public string? Imagen { get; set; }

        public int IdCategoria { get; set; }

        public string? CategoriaNombre { get; set; }

        public bool Activo { get; set; }
    }
}