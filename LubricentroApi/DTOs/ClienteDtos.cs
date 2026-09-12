using System.ComponentModel.DataAnnotations;

namespace LubricentroApi.DTOs
{
    // Datos necesarios para crear un cliente.
    public class CrearClienteDto
    {
        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Apellido { get; set; }

        [MaxLength(20)]
        public string? CUIL { get; set; }

        [MaxLength(30)]
        public string? Telefono { get; set; }

        [MaxLength(150)]
        [EmailAddress]
        public string? Email { get; set; }
    }

    // Datos permitidos para editar un cliente.
    public class EditarClienteDto
    {
        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Apellido { get; set; }

        [MaxLength(20)]
        public string? CUIL { get; set; }

        [MaxLength(30)]
        public string? Telefono { get; set; }

        [MaxLength(150)]
        [EmailAddress]
        public string? Email { get; set; }
    }

    // Información que devolverá la API.
    public class ClienteResponseDto
    {
        public int IdCliente { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public string? Apellido { get; set; }

        public string? CUIL { get; set; }

        public string? Telefono { get; set; }

        public string? Email { get; set; }

        public bool Activo { get; set; }
    }
}