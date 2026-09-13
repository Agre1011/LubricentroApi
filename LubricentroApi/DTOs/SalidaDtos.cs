using System.ComponentModel.DataAnnotations;

namespace LubricentroApi.DTOs
{
    // Cada producto incluido en una venta.
    public class CrearDetalleSalidaDto
    {
        [Range(1, int.MaxValue)]
        public int IdProducto { get; set; }

        [Range(1, int.MaxValue)]
        public int Cantidad { get; set; }
    }

    // Datos necesarios para registrar una venta.
    public class CrearSalidaDto
    {
        [Range(1, int.MaxValue)]
        public int IdCliente { get; set; }

        [Required]
        [MaxLength(30)]
        public string MedioPago { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? EntidadPago { get; set; }

        [MaxLength(500)]
        public string? Observacion { get; set; }

        [Required]
        [MinLength(1)]
        public List<CrearDetalleSalidaDto> Detalles { get; set; } = new();
    }

    // Detalle que devolverá la API.
    public class DetalleSalidaResponseDto
    {
        public int IdProducto { get; set; }

        public string Producto { get; set; } = string.Empty;

        public string Marca { get; set; } = string.Empty;

        public string Variante { get; set; } = string.Empty;

        public int Cantidad { get; set; }

        public decimal PrecioVentaUnitario { get; set; }
    }

    // Respuesta completa de una venta.
    public class SalidaResponseDto
    {
        public int IdSalida { get; set; }

        public DateTime FechaHora { get; set; }

        public int IdCliente { get; set; }

        public string Cliente { get; set; } = string.Empty;

        public int IdUsuario { get; set; }

        public string Usuario { get; set; } = string.Empty;

        public string MedioPago { get; set; } = string.Empty;

        public string? EntidadPago { get; set; }

        public string? Observacion { get; set; }

        public List<DetalleSalidaResponseDto> Detalles { get; set; } = new();
    }

    // Información resumida para el historial de ventas.
    public class SalidaListadoDto
    {
        public int IdSalida { get; set; }

        public DateTime FechaHora { get; set; }

        public int IdCliente { get; set; }

        public string Cliente { get; set; } = string.Empty;

        public int IdUsuario { get; set; }

        public string Usuario { get; set; } = string.Empty;

        public string MedioPago { get; set; } = string.Empty;

        public string? EntidadPago { get; set; }

        public string? Observacion { get; set; }
    }
}