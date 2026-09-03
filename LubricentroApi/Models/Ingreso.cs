using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LubricentroApi.Models
{
    public class Ingreso
    {
        [Key]
        public int IdIngreso { get; set; }

        public DateTime FechaHora { get; set; } = DateTime.Now;

        public int IdProveedor { get; set; }

        public int IdUsuario { get; set; }

        [MaxLength(300)]
        public string? Observacion { get; set; }

        // Relación: cada ingreso pertenece a un proveedor.
        [ForeignKey(nameof(IdProveedor))]
        public Proveedor? Proveedor { get; set; }

        // Relación: cada ingreso es registrado por un usuario.
        [ForeignKey(nameof(IdUsuario))]
        public Usuario? Usuario { get; set; }

        // Relación: un ingreso puede tener muchos detalles.
        public ICollection<DetalleIngreso> Detalles { get; set; } = new List<DetalleIngreso>();
    }
}