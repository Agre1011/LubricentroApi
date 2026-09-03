using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LubricentroApi.Models
{
    public class Salida
    {
        [Key]
        public int IdSalida { get; set; }

        public DateTime FechaHora { get; set; } = DateTime.Now;

        public int IdCliente { get; set; }

        public int IdUsuario { get; set; }

        [Required]
        [MaxLength(30)]
        public string MedioPago { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? EntidadPago { get; set; }

        [MaxLength(300)]
        public string? Observacion { get; set; }

        // Relación: cada salida pertenece a un cliente.
        [ForeignKey(nameof(IdCliente))]
        public Cliente? Cliente { get; set; }

        // Relación: cada salida es registrada por un usuario.
        [ForeignKey(nameof(IdUsuario))]
        public Usuario? Usuario { get; set; }

        // Relación: una salida puede tener muchos detalles.
        public ICollection<DetalleSalida> Detalles { get; set; }
            = new List<DetalleSalida>();
    }
}