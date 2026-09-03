using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LubricentroApi.Models
{
    public class DetalleIngreso
    {
        [Key]
        public int IdDetalleIngreso { get; set; }

        public int IdIngreso { get; set; }

        public int IdProducto { get; set; }

        public int Cantidad { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecioCompraUnitario { get; set; }

        // Relación: cada detalle pertenece a un ingreso.
        [ForeignKey(nameof(IdIngreso))]
        public Ingreso? Ingreso { get; set; }

        // Relación: cada detalle corresponde a un producto.
        [ForeignKey(nameof(IdProducto))]
        public Producto? Producto { get; set; }
    }
}