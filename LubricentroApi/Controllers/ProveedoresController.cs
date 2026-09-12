using LubricentroApi.Data;
using LubricentroApi.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LubricentroApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Empleado")]
    public class ProveedoresController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProveedoresController(AppDbContext context)
        {
            _context = context;
        }

        // ----------------------------------------------------
        // LISTAR PROVEEDORES CON PAGINACIÓN
        // GET: api/proveedores?pagina=1&tamanoPagina=5
        // Admin y Empleado
        // ----------------------------------------------------
        [HttpGet]
        public async Task<ActionResult<RespuestaPaginadaDto<ProveedorResponseDto>>> GetProveedores(
            [FromQuery] ParametrosPaginacionDto parametros)
        {
            var consulta = _context.Proveedores
                .AsNoTracking()
                .OrderBy(p => p.IdProveedor);

            int totalRegistros = await consulta.CountAsync();

            int totalPaginas = (int)Math.Ceiling(
                totalRegistros / (double)parametros.TamanoPagina
            );

            var proveedores = await consulta
                .Skip((parametros.Pagina - 1) * parametros.TamanoPagina)
                .Take(parametros.TamanoPagina)
                .Select(p => new ProveedorResponseDto
                {
                    IdProveedor = p.IdProveedor,
                    Nombre = p.Nombre,
                    CUIT = p.CUIT,
                    Telefono = p.Telefono,
                    Email = p.Email,
                    Activo = p.Activo
                })
                .ToListAsync();

            var respuesta = new RespuestaPaginadaDto<ProveedorResponseDto>
            {
                PaginaActual = parametros.Pagina,
                TamanoPagina = parametros.TamanoPagina,
                TotalRegistros = totalRegistros,
                TotalPaginas = totalPaginas,
                TienePaginaAnterior = parametros.Pagina > 1,
                TienePaginaSiguiente = parametros.Pagina < totalPaginas,
                Datos = proveedores
            };

            return Ok(respuesta);
        }

        // ----------------------------------------------------
        // OBTENER PROVEEDOR POR ID
        // GET: api/proveedores/1
        // Admin y Empleado
        // ----------------------------------------------------
        [HttpGet("{id}")]
        public async Task<ActionResult<ProveedorResponseDto>> GetProveedor(int id)
        {
            var proveedor = await _context.Proveedores
                .AsNoTracking()
                .Where(p => p.IdProveedor == id)
                .Select(p => new ProveedorResponseDto
                {
                    IdProveedor = p.IdProveedor,
                    Nombre = p.Nombre,
                    CUIT = p.CUIT,
                    Telefono = p.Telefono,
                    Email = p.Email,
                    Activo = p.Activo
                })
                .FirstOrDefaultAsync();

            if (proveedor == null)
            {
                return NotFound(new
                {
                    mensaje = "Proveedor no encontrado."
                });
            }

            return Ok(proveedor);
        }

        // ----------------------------------------------------
        // CREAR PROVEEDOR
        // POST: api/proveedores
        // Solo Admin
        // ----------------------------------------------------
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ProveedorResponseDto>> CrearProveedor(
            CrearProveedorDto dto)
        {
            var proveedor = new Models.Proveedor
            {
                Nombre = dto.Nombre,
                CUIT = dto.CUIT,
                Telefono = dto.Telefono,
                Email = dto.Email,
                Activo = true
            };

            _context.Proveedores.Add(proveedor);

            await _context.SaveChangesAsync();

            var respuesta = new ProveedorResponseDto
            {
                IdProveedor = proveedor.IdProveedor,
                Nombre = proveedor.Nombre,
                CUIT = proveedor.CUIT,
                Telefono = proveedor.Telefono,
                Email = proveedor.Email,
                Activo = proveedor.Activo
            };

            return CreatedAtAction(
                nameof(GetProveedor),
                new { id = proveedor.IdProveedor },
                respuesta
            );
        }

        // ----------------------------------------------------
        // EDITAR PROVEEDOR
        // PUT: api/proveedores/{id}
        // Solo Admin
        // ----------------------------------------------------
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditarProveedor(
            int id,
            EditarProveedorDto dto)
        {
            var proveedor = await _context.Proveedores
                .FirstOrDefaultAsync(p => p.IdProveedor == id);

            if (proveedor == null)
            {
                return NotFound(new
                {
                    mensaje = "Proveedor no encontrado."
                });
            }

            proveedor.Nombre = dto.Nombre;
            proveedor.CUIT = dto.CUIT;
            proveedor.Telefono = dto.Telefono;
            proveedor.Email = dto.Email;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Proveedor actualizado correctamente.",
                idProveedor = proveedor.IdProveedor
            });
        }


        // ----------------------------------------------------
        // DAR DE BAJA PROVEEDOR
        // DELETE: api/proveedores/{id}
        // Solo Admin
        // ----------------------------------------------------
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EliminarProveedor(int id)
        {
            var proveedor = await _context.Proveedores
                .FirstOrDefaultAsync(p => p.IdProveedor == id);

            if (proveedor == null)
            {
                return NotFound(new
                {
                    mensaje = "Proveedor no encontrado."
                });
            }

            if (!proveedor.Activo)
            {
                return BadRequest(new
                {
                    mensaje = "El proveedor ya se encuentra dado de baja."
                });
            }

            // Baja lógica: no se elimina físicamente de la base de datos.
            proveedor.Activo = false;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Proveedor dado de baja correctamente.",
                idProveedor = proveedor.IdProveedor
            });
        }
    }
}