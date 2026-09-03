using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LubricentroApi.Models
{
    public class DetalleSalida
    {
        [Key]
        public int IdDetalleSalida { get; set; }

        public int IdSalida { get; set; }

        public int IdProducto { get; set; }

        public int Cantidad { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecioVentaUnitario { get; set; }

        // Relación: cada detalle pertenece a una salida o venta.
        [ForeignKey(nameof(IdSalida))]
        public Salida? Salida { get; set; }

        // Relación: cada detalle corresponde a un producto.
        [ForeignKey(nameof(IdProducto))]
        public Producto? Producto { get; set; }
    }
}