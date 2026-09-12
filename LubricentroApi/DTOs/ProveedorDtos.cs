using System.ComponentModel.DataAnnotations;

namespace LubricentroApi.DTOs
{
    // Datos necesarios para crear un proveedor.
    public class CrearProveedorDto
    {
        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string CUIT { get; set; } = string.Empty;

        [MaxLength(30)]
        public string? Telefono { get; set; }

        [MaxLength(150)]
        [EmailAddress]
        public string? Email { get; set; }
    }

    // Datos permitidos para editar un proveedor.
    public class EditarProveedorDto
    {
        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string CUIT { get; set; } = string.Empty;

        [MaxLength(30)]
        public string? Telefono { get; set; }

        [MaxLength(150)]
        [EmailAddress]
        public string? Email { get; set; }
    }

    // Información que devolverá la API.
    public class ProveedorResponseDto
    {
        public int IdProveedor { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public string CUIT { get; set; } = string.Empty;

        public string? Telefono { get; set; }

        public string? Email { get; set; }

        public bool Activo { get; set; }
    }
}