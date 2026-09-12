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
    public class ClientesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ClientesController(AppDbContext context)
        {
            _context = context;
        }

        // ----------------------------------------------------
        // LISTAR CLIENTES CON PAGINACIÓN
        // GET: api/clientes?pagina=1&tamanoPagina=10
        // Admin y Empleado
        // ----------------------------------------------------
        [HttpGet]
        public async Task<ActionResult<RespuestaPaginadaDto<ClienteResponseDto>>> GetClientes(
            [FromQuery] ParametrosPaginacionDto parametros)
        {
            var consulta = _context.Clientes
                .AsNoTracking()
                .OrderBy(c => c.IdCliente);

            int totalRegistros = await consulta.CountAsync();

            int totalPaginas = (int)Math.Ceiling(
                totalRegistros / (double)parametros.TamanoPagina
            );

            var clientes = await consulta
                .Skip((parametros.Pagina - 1) * parametros.TamanoPagina)
                .Take(parametros.TamanoPagina)
                .Select(c => new ClienteResponseDto
                {
                    IdCliente = c.IdCliente,
                    Nombre = c.Nombre,
                    Apellido = c.Apellido,
                    CUIL = c.CUIL,
                    Telefono = c.Telefono,
                    Email = c.Email,
                    Activo = c.Activo
                })
                .ToListAsync();

            var respuesta = new RespuestaPaginadaDto<ClienteResponseDto>
            {
                PaginaActual = parametros.Pagina,
                TamanoPagina = parametros.TamanoPagina,
                TotalRegistros = totalRegistros,
                TotalPaginas = totalPaginas,
                TienePaginaAnterior = parametros.Pagina > 1,
                TienePaginaSiguiente = parametros.Pagina < totalPaginas,
                Datos = clientes
            };

            return Ok(respuesta);
        }

        // ----------------------------------------------------
        // OBTENER CLIENTE POR ID
        // GET: api/clientes/1
        // Admin y Empleado
        // ----------------------------------------------------
        [HttpGet("{id}")]
        public async Task<ActionResult<ClienteResponseDto>> GetCliente(int id)
        {
            var cliente = await _context.Clientes
                .AsNoTracking()
                .Where(c => c.IdCliente == id)
                .Select(c => new ClienteResponseDto
                {
                    IdCliente = c.IdCliente,
                    Nombre = c.Nombre,
                    Apellido = c.Apellido,
                    CUIL = c.CUIL,
                    Telefono = c.Telefono,
                    Email = c.Email,
                    Activo = c.Activo
                })
                .FirstOrDefaultAsync();

            if (cliente == null)
            {
                return NotFound(new
                {
                    mensaje = "Cliente no encontrado."
                });
            }

            return Ok(cliente);
        }

        // ----------------------------------------------------
        // CREAR CLIENTE
        // POST: api/clientes
        // Admin y Empleado
        // ----------------------------------------------------
        [HttpPost]
        public async Task<ActionResult<ClienteResponseDto>> CrearCliente(
            CrearClienteDto dto)
        {
            var cliente = new Models.Cliente
            {
                Nombre = dto.Nombre,
                Apellido = dto.Apellido,
                CUIL = dto.CUIL,
                Telefono = dto.Telefono,
                Email = dto.Email,
                Activo = true
            };

            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();

            var respuesta = new ClienteResponseDto
            {
                IdCliente = cliente.IdCliente,
                Nombre = cliente.Nombre,
                Apellido = cliente.Apellido,
                CUIL = cliente.CUIL,
                Telefono = cliente.Telefono,
                Email = cliente.Email,
                Activo = cliente.Activo
            };

            return CreatedAtAction(
                nameof(GetCliente),
                new { id = cliente.IdCliente },
                respuesta
            );
        }

        // ----------------------------------------------------
        // EDITAR CLIENTE
        // PUT: api/clientes/{id}
        // Admin y Empleado
        // ----------------------------------------------------
        [HttpPut("{id}")]
        public async Task<IActionResult> EditarCliente(
            int id,
            EditarClienteDto dto)
        {
            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(c => c.IdCliente == id);

            if (cliente == null)
            {
                return NotFound(new
                {
                    mensaje = "Cliente no encontrado."
                });
            }

            cliente.Nombre = dto.Nombre;
            cliente.Apellido = dto.Apellido;
            cliente.CUIL = dto.CUIL;
            cliente.Telefono = dto.Telefono;
            cliente.Email = dto.Email;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Cliente actualizado correctamente.",
                idCliente = cliente.IdCliente
            });
        }
        // ----------------------------------------------------
        // DAR DE BAJA CLIENTE
        // DELETE: api/clientes/{id}
        // Admin y Empleado
        // ----------------------------------------------------
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarCliente(int id)
        {
            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(c => c.IdCliente == id);

            if (cliente == null)
            {
                return NotFound(new
                {
                    mensaje = "Cliente no encontrado."
                });
            }

            // Consumidor Final no se puede dar de baja.
            if (cliente.IdCliente == 1)
            {
                return BadRequest(new
                {
                    mensaje = "El cliente Consumidor Final no puede darse de baja."
                });
            }

            if (!cliente.Activo)
            {
                return BadRequest(new
                {
                    mensaje = "El cliente ya se encuentra dado de baja."
                });
            }

            // Baja lógica.
            cliente.Activo = false;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Cliente dado de baja correctamente.",
                idCliente = cliente.IdCliente
            });
        }
    }
}