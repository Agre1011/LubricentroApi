using LubricentroApi.Data;
using LubricentroApi.DTOs;
using LubricentroApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LubricentroApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class IngresosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public IngresosController(AppDbContext context)
        {
            _context = context;
        }

        // ----------------------------------------------------
        // REGISTRAR COMPRA / INGRESO
        // POST: api/ingresos
        // Solo Admin
        // ----------------------------------------------------
        [HttpPost]
        public async Task<ActionResult<IngresoResponseDto>> CrearIngreso(
            CrearIngresoDto dto)
        {
            // Obtener el usuario desde el JWT.
            var idUsuarioTexto = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(idUsuarioTexto, out int idUsuario))
            {
                return Unauthorized(new
                {
                    mensaje = "No se pudo identificar al usuario."
                });
            }

            // Verificar proveedor.
            var proveedor = await _context.Proveedores
                .FirstOrDefaultAsync(p => p.IdProveedor == dto.IdProveedor);

            if (proveedor == null)
            {
                return BadRequest(new
                {
                    mensaje = "El proveedor indicado no existe."
                });
            }

            if (!proveedor.Activo)
            {
                return BadRequest(new
                {
                    mensaje = "El proveedor se encuentra dado de baja."
                });
            }

            // Evitamos repetir el mismo producto dentro de una misma compra.
            bool hayProductosRepetidos = dto.Detalles
                .GroupBy(d => d.IdProducto)
                .Any(g => g.Count() > 1);

            if (hayProductosRepetidos)
            {
                return BadRequest(new
                {
                    mensaje = "No se puede repetir el mismo producto dentro de una compra."
                });
            }

            // Obtener los productos enviados.
            var idsProductos = dto.Detalles
                .Select(d => d.IdProducto)
                .ToList();

            var productos = await _context.Productos
                .Where(p => idsProductos.Contains(p.IdProducto))
                .ToListAsync();

            // Verificar que todos existan.
            var idsEncontrados = productos
                .Select(p => p.IdProducto)
                .ToList();

            var idsNoEncontrados = idsProductos
                .Except(idsEncontrados)
                .ToList();

            if (idsNoEncontrados.Count > 0)
            {
                return BadRequest(new
                {
                    mensaje = "Uno o más productos no existen.",
                    productosNoEncontrados = idsNoEncontrados
                });
            }

            // Verificar que ninguno esté dado de baja.
            var productosInactivos = productos
                .Where(p => !p.Activo)
                .Select(p => p.IdProducto)
                .ToList();

            if (productosInactivos.Count > 0)
            {
                return BadRequest(new
                {
                    mensaje = "Uno o más productos se encuentran dados de baja.",
                    productosInactivos
                });
            }

            // Usamos una transacción para que toda la compra se guarde junta.
            await using var transaccion =
                await _context.Database.BeginTransactionAsync();

            try
            {
                var ingreso = new Ingreso
                {
                    FechaHora = DateTime.Now,
                    IdProveedor = dto.IdProveedor,
                    IdUsuario = idUsuario,
                    Observacion = dto.Observacion
                };

                _context.Ingresos.Add(ingreso);

                // Guardamos primero para obtener IdIngreso.
                await _context.SaveChangesAsync();

                foreach (var detalleDto in dto.Detalles)
                {
                    var producto = productos
                        .First(p => p.IdProducto == detalleDto.IdProducto);

                    var detalle = new DetalleIngreso
                    {
                        IdIngreso = ingreso.IdIngreso,
                        IdProducto = producto.IdProducto,
                        Cantidad = detalleDto.Cantidad,
                        PrecioCompraUnitario = detalleDto.PrecioCompraUnitario
                    };

                    _context.Set<DetalleIngreso>().Add(detalle);

                    // La compra aumenta el stock.
                    producto.Stock += detalleDto.Cantidad;

                    // Guardamos también el último precio de compra.
                    producto.PrecioCompra = detalleDto.PrecioCompraUnitario;
                }

                await _context.SaveChangesAsync();

                await transaccion.CommitAsync();

                var respuesta = new IngresoResponseDto
                {
                    IdIngreso = ingreso.IdIngreso,
                    FechaHora = ingreso.FechaHora,
                    IdProveedor = proveedor.IdProveedor,
                    Proveedor = proveedor.Nombre,
                    IdUsuario = idUsuario,
                    Usuario = User.Identity?.Name ?? string.Empty,
                    Observacion = ingreso.Observacion,
                    Detalles = dto.Detalles.Select(d =>
                    {
                        var producto = productos
                            .First(p => p.IdProducto == d.IdProducto);

                        return new DetalleIngresoResponseDto
                        {
                            IdProducto = producto.IdProducto,
                            Producto = producto.Nombre,
                            Marca = producto.Marca,
                            Variante = producto.Variante,
                            Cantidad = d.Cantidad,
                            PrecioCompraUnitario = d.PrecioCompraUnitario
                        };
                    }).ToList()
                };

                return StatusCode(StatusCodes.Status201Created, respuesta);
            }
            catch
            {
                await transaccion.RollbackAsync();

                return StatusCode(500, new
                {
                    mensaje = "Ocurrió un error al registrar la compra."
                });
            }
        }
    }
}