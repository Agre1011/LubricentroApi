using System.ComponentModel.DataAnnotations;

namespace LubricentroApi.Models
{
    public class Categoria
    {
        [Key]
        public int IdCategoria { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [MaxLength(250)]
        public string? Descripcion { get; set; }

        // Relación: una categoría puede tener muchos productos.
        public ICollection<Producto> Productos { get; set; } = new List<Producto>();
    }
}