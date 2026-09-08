using LubricentroApi.Data;
using LubricentroApi.DTOs;
using LubricentroApi.Models;
using LubricentroApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LubricentroApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly JwtService _jwtService;

        public AuthController(
            AppDbContext context,
            JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        // ----------------------------------------------------
        // LOGIN
        // POST: api/auth/login
        // ----------------------------------------------------
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login(
            LoginDto dto)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Username == dto.Username);

            // El usuario debe existir.
            if (usuario == null)
            {
                return Unauthorized(new
                {
                    mensaje = "Usuario o contraseña incorrectos."
                });
            }

            // La cuenta debe estar activa.
            if (!usuario.Activo)
            {
                return Unauthorized(new
                {
                    mensaje = "El usuario se encuentra inactivo."
                });
            }

            // Verificamos la contraseña contra el hash almacenado.
            bool passwordCorrecta =
                BCrypt.Net.BCrypt.Verify(
                    dto.Password,
                    usuario.PasswordHash
                );

            if (!passwordCorrecta)
            {
                return Unauthorized(new
                {
                    mensaje = "Usuario o contraseña incorrectos."
                });
            }

            // Generamos el token JWT.
            var resultado =
                _jwtService.GenerarToken(usuario);

            return Ok(new AuthResponseDto
            {
                Token = resultado.Token,
                Username = usuario.Username,
                Rol = usuario.Rol,
                Expiracion = resultado.Expiracion
            });
        }

        // ----------------------------------------------------
        // CREAR USUARIO
        // POST: api/auth/usuarios
        // Solo Admin
        // ----------------------------------------------------
        [Authorize(Roles = "Admin")]
        [HttpPost("usuarios")]
        public async Task<IActionResult> CrearUsuario(
            CrearUsuarioDto dto)
        {
            // Validamos los roles permitidos.
            if (dto.Rol != "Admin" &&
                dto.Rol != "Empleado")
            {
                return BadRequest(new
                {
                    mensaje = "El rol debe ser Admin o Empleado."
                });
            }

            // No permitimos usernames repetidos.
            bool usernameExiste = await _context.Usuarios
                .AnyAsync(u => u.Username == dto.Username);

            if (usernameExiste)
            {
                return BadRequest(new
                {
                    mensaje = "El nombre de usuario ya existe."
                });
            }

            var usuario = new Usuario
            {
                Nombre = dto.Nombre,
                Apellido = dto.Apellido,
                Username = dto.Username,
                Email = dto.Email,

                PasswordHash =
                    BCrypt.Net.BCrypt.HashPassword(dto.Password),

                Rol = dto.Rol,
                Activo = true
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return StatusCode(201, new
            {
                mensaje = "Usuario creado correctamente.",
                idUsuario = usuario.IdUsuario,
                username = usuario.Username,
                rol = usuario.Rol
            });
        }
    }
}