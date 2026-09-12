using System.ComponentModel.DataAnnotations;

namespace LubricentroApi.DTOs
{
    // Datos necesarios para crear una categoría.
    public class CrearCategoriaDto
    {
        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [MaxLength(250)]
        public string? Descripcion { get; set; }
    }

    // Datos permitidos para editar una categoría.
    public class EditarCategoriaDto
    {
        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [MaxLength(250)]
        public string? Descripcion { get; set; }
    }

    // Datos que devolverá la API.
    public class CategoriaResponseDto
    {
        public int IdCategoria { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public string? Descripcion { get; set; }
    }
}