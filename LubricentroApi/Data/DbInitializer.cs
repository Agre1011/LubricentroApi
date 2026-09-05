using LubricentroApi.Models;
using Microsoft.EntityFrameworkCore;

namespace LubricentroApi.Data
{
    public static class DbInitializer
    {
        public static async Task InicializarAsync(AppDbContext context)
        {
            // Comprobamos si ya existe algún usuario.
            if (await context.Usuarios.AnyAsync())
            {
                return;
            }

            // Creamos el administrador inicial.
            var admin = new Usuario
            {
                Nombre = "Administrador",
                Apellido = "Lubricentro",
                Username = "admin",
                Email = "admin@lubricentro.com",

                // La contraseña nunca se guarda directamente.
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),

                Rol = "Admin",
                Activo = true
            };

            context.Usuarios.Add(admin);

            await context.SaveChangesAsync();
        }
    }
}