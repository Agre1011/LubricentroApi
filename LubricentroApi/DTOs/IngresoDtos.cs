using System.ComponentModel.DataAnnotations;

namespace LubricentroApi.DTOs
{
    // Representa cada producto incluido dentro de una compra.
    public class CrearDetalleIngresoDto
    {
        [Range(1, int.MaxValue)]
        public int IdProducto { get; set; }

        [Range(1, int.MaxValue)]
        public int Cantidad { get; set; }

        [Range(
    typeof(decimal),
    "0.01",
    "999999999",
    ParseLimitsInInvariantCulture = true,
    ConvertValueInInvariantCulture = true)]
        public decimal PrecioCompraUnitario { get; set; }
    }

    // Datos que recibirá la API al registrar una compra.
    public class CrearIngresoDto
    {
        [Range(1, int.MaxValue)]
        public int IdProveedor { get; set; }

        [MaxLength(500)]
        public string? Observacion { get; set; }

        [Required]
        [MinLength(1)]
        public List<CrearDetalleIngresoDto> Detalles { get; set; } = new();
    }

    // Información de cada producto que devolverá la API.
    public class DetalleIngresoResponseDto
    {
        public int IdProducto { get; set; }

        public string Producto { get; set; } = string.Empty;

        public string Marca { get; set; } = string.Empty;

        public string Variante { get; set; } = string.Empty;

        public int Cantidad { get; set; }

        public decimal PrecioCompraUnitario { get; set; }
    }

    // Información completa de una compra.
    public class IngresoResponseDto
    {
        public int IdIngreso { get; set; }

        public DateTime FechaHora { get; set; }

        public int IdProveedor { get; set; }

        public string Proveedor { get; set; } = string.Empty;

        public int IdUsuario { get; set; }

        public string Usuario { get; set; } = string.Empty;

        public string? Observacion { get; set; }

        public List<DetalleIngresoResponseDto> Detalles { get; set; } = new();
    }
}