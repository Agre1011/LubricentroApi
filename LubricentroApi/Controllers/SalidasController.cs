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
    [Authorize(Roles = "Admin,Empleado")]
    public class SalidasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SalidasController(AppDbContext context)
        {
            _context = context;
        }

        // ----------------------------------------------------
        // LISTAR VENTAS CON PAGINACIÓN
        // GET: api/salidas?pagina=1&tamanoPagina=10
        // Admin y Empleado
        // ----------------------------------------------------
        [HttpGet]
        public async Task<ActionResult<RespuestaPaginadaDto<SalidaListadoDto>>> GetSalidas(
            [FromQuery] ParametrosPaginacionDto parametros)
        {
            var consulta =
                from salida in _context.Salidas.AsNoTracking()
                join cliente in _context.Clientes.AsNoTracking()
                    on salida.IdCliente equals cliente.IdCliente
                join usuario in _context.Usuarios.AsNoTracking()
                    on salida.IdUsuario equals usuario.IdUsuario
                orderby salida.FechaHora descending
                select new SalidaListadoDto
                {
                    IdSalida = salida.IdSalida,
                    FechaHora = salida.FechaHora,
                    IdCliente = cliente.IdCliente,

                    Cliente = cliente.Apellido != null && cliente.Apellido != ""
                        ? cliente.Nombre + " " + cliente.Apellido
                        : cliente.Nombre,

                    IdUsuario = usuario.IdUsuario,
                    Usuario = usuario.Username,
                    MedioPago = salida.MedioPago,
                    EntidadPago = salida.EntidadPago,
                    Observacion = salida.Observacion
                };

            int totalRegistros = await consulta.CountAsync();

            int totalPaginas = (int)Math.Ceiling(
                totalRegistros / (double)parametros.TamanoPagina
            );

            var salidas = await consulta
                .Skip((parametros.Pagina - 1) * parametros.TamanoPagina)
                .Take(parametros.TamanoPagina)
                .ToListAsync();

            var respuesta = new RespuestaPaginadaDto<SalidaListadoDto>
            {
                PaginaActual = parametros.Pagina,
                TamanoPagina = parametros.TamanoPagina,
                TotalRegistros = totalRegistros,
                TotalPaginas = totalPaginas,
                TienePaginaAnterior = parametros.Pagina > 1,
                TienePaginaSiguiente = parametros.Pagina < totalPaginas,
                Datos = salidas
            };

            return Ok(respuesta);
        }


        // ----------------------------------------------------
        // OBTENER VENTA COMPLETA POR ID
        // GET: api/salidas/{id}
        // Admin y Empleado
        // ----------------------------------------------------
        [HttpGet("{id}")]
        public async Task<ActionResult<SalidaResponseDto>> GetSalida(int id)
        {
            var salida = await _context.Salidas
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.IdSalida == id);

            if (salida == null)
            {
                return NotFound(new
                {
                    mensaje = "Venta no encontrada."
                });
            }

            var cliente = await _context.Clientes
                .AsNoTracking()
                .FirstAsync(c => c.IdCliente == salida.IdCliente);

            var usuario = await _context.Usuarios
                .AsNoTracking()
                .FirstAsync(u => u.IdUsuario == salida.IdUsuario);

            var detalles = await (
                from detalle in _context.DetalleSalidas.AsNoTracking()
                join producto in _context.Productos.AsNoTracking()
                    on detalle.IdProducto equals producto.IdProducto
                where detalle.IdSalida == id
                select new DetalleSalidaResponseDto
                {
                    IdProducto = producto.IdProducto,
                    Producto = producto.Nombre,
                    Marca = producto.Marca,
                    Variante = producto.Variante,
                    Cantidad = detalle.Cantidad,
                    PrecioVentaUnitario = detalle.PrecioVentaUnitario
                }
            ).ToListAsync();

            var nombreCliente = string.IsNullOrWhiteSpace(cliente.Apellido)
                ? cliente.Nombre
                : $"{cliente.Nombre} {cliente.Apellido}";

            var respuesta = new SalidaResponseDto
            {
                IdSalida = salida.IdSalida,
                FechaHora = salida.FechaHora,
                IdCliente = cliente.IdCliente,
                Cliente = nombreCliente,
                IdUsuario = usuario.IdUsuario,
                Usuario = usuario.Username,
                MedioPago = salida.MedioPago,
                EntidadPago = salida.EntidadPago,
                Observacion = salida.Observacion,
                Detalles = detalles
            };

            return Ok(respuesta);
        }





        // ----------------------------------------------------
        // REGISTRAR VENTA / SALIDA
        // POST: api/salidas
        // Admin y Empleado
        // ----------------------------------------------------
        [HttpPost]
        public async Task<ActionResult<SalidaResponseDto>> CrearSalida(
            CrearSalidaDto dto)
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

            // Verificar cliente.
            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(c => c.IdCliente == dto.IdCliente);

            if (cliente == null)
            {
                return BadRequest(new
                {
                    mensaje = "El cliente indicado no existe."
                });
            }

            if (!cliente.Activo)
            {
                return BadRequest(new
                {
                    mensaje = "El cliente se encuentra dado de baja."
                });
            }

            // Validar medio de pago.
            var mediosPagoPermitidos = new[]
            {
                "Efectivo",
                "Tarjeta",
                "Transferencia"
            };

            bool medioPagoValido = mediosPagoPermitidos.Any(m =>
                m.Equals(dto.MedioPago, StringComparison.OrdinalIgnoreCase));

            if (!medioPagoValido)
            {
                return BadRequest(new
                {
                    mensaje = "Medio de pago inválido. Debe ser Efectivo, Tarjeta o Transferencia."
                });
            }

            // Evitar productos repetidos en la misma venta.
            bool hayProductosRepetidos = dto.Detalles
                .GroupBy(d => d.IdProducto)
                .Any(g => g.Count() > 1);

            if (hayProductosRepetidos)
            {
                return BadRequest(new
                {
                    mensaje = "No se puede repetir el mismo producto dentro de una venta."
                });
            }

            // Obtener los productos enviados.
            var idsProductos = dto.Detalles
                .Select(d => d.IdProducto)
                .ToList();

            var productos = await _context.Productos
                .Where(p => idsProductos.Contains(p.IdProducto))
                .ToListAsync();

            // Verificar productos inexistentes.
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

            // Verificar productos dados de baja.
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

            // Verificar el stock de TODOS los productos antes de vender.
            var faltantes = new List<object>();

            foreach (var detalleDto in dto.Detalles)
            {
                var producto = productos
                    .First(p => p.IdProducto == detalleDto.IdProducto);

                if (producto.Stock < detalleDto.Cantidad)
                {
                    faltantes.Add(new
                    {
                        idProducto = producto.IdProducto,
                        producto = $"{producto.Marca} {producto.Variante}",
                        cantidadSolicitada = detalleDto.Cantidad,
                        stockDisponible = producto.Stock,
                        cantidadFaltante = detalleDto.Cantidad - producto.Stock
                    });
                }
            }

            // Si algún producto no tiene stock suficiente,
            // rechazamos TODA la venta.
            if (faltantes.Count > 0)
            {
                return BadRequest(new
                {
                    mensaje = "No hay stock suficiente para completar la venta.",
                    productosConStockInsuficiente = faltantes
                });
            }

            // Transacción: o se guarda toda la venta o no se guarda nada.
            await using var transaccion =
                await _context.Database.BeginTransactionAsync();

            try
            {
                var salida = new Salida
                {
                    FechaHora = DateTime.Now,
                    IdCliente = dto.IdCliente,
                    IdUsuario = idUsuario,
                    MedioPago = dto.MedioPago,
                    EntidadPago = dto.EntidadPago,
                    Observacion = dto.Observacion
                };

                _context.Salidas.Add(salida);

                // Guardamos para obtener el IdSalida.
                await _context.SaveChangesAsync();

                foreach (var detalleDto in dto.Detalles)
                {
                    var producto = productos
                        .First(p => p.IdProducto == detalleDto.IdProducto);

                    var detalle = new DetalleSalida
                    {
                        IdSalida = salida.IdSalida,
                        IdProducto = producto.IdProducto,
                        Cantidad = detalleDto.Cantidad,

                        // El precio se toma del producto.
                        PrecioVentaUnitario = producto.PrecioVenta
                    };

                    _context.Set<DetalleSalida>().Add(detalle);

                    // La venta descuenta stock.
                    producto.Stock -= detalleDto.Cantidad;
                }

                await _context.SaveChangesAsync();

                await transaccion.CommitAsync();

                var nombreCliente = string.IsNullOrWhiteSpace(cliente.Apellido)
                    ? cliente.Nombre
                    : $"{cliente.Nombre} {cliente.Apellido}";

                var respuesta = new SalidaResponseDto
                {
                    IdSalida = salida.IdSalida,
                    FechaHora = salida.FechaHora,
                    IdCliente = cliente.IdCliente,
                    Cliente = nombreCliente,
                    IdUsuario = idUsuario,
                    Usuario = User.Identity?.Name ?? string.Empty,
                    MedioPago = salida.MedioPago,
                    EntidadPago = salida.EntidadPago,
                    Observacion = salida.Observacion,

                    Detalles = dto.Detalles.Select(d =>
                    {
                        var producto = productos
                            .First(p => p.IdProducto == d.IdProducto);

                        return new DetalleSalidaResponseDto
                        {
                            IdProducto = producto.IdProducto,
                            Producto = producto.Nombre,
                            Marca = producto.Marca,
                            Variante = producto.Variante,
                            Cantidad = d.Cantidad,
                            PrecioVentaUnitario = producto.PrecioVenta
                        };
                    }).ToList()
                };

                return StatusCode(
                    StatusCodes.Status201Created,
                    respuesta
                );
            }
            catch
            {
                await transaccion.RollbackAsync();

                return StatusCode(500, new
                {
                    mensaje = "Ocurrió un error al registrar la venta."
                });
            }
        }
    }
}