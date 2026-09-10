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
    public class ProductosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductosController(AppDbContext context)
        {
            _context = context;
        }

        // ----------------------------------------------------
        // LISTAR PRODUCTOS
        // GET: api/productos
        // Admin y Empleado
        // ----------------------------------------------------
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductoResponseDto>>> GetProductos()
        {
            var productos = await _context.Productos
                .AsNoTracking()
                .Select(p => new ProductoResponseDto
                {
                    IdProducto = p.IdProducto,
                    Nombre = p.Nombre,
                    Marca = p.Marca,
                    Variante = p.Variante,
                    PrecioCompra = p.PrecioCompra,
                    PrecioVenta = p.PrecioVenta,
                    Stock = p.Stock,
                    Imagen = p.Imagen,
                    IdCategoria = p.IdCategoria,
                    CategoriaNombre = p.Categoria != null
                        ? p.Categoria.Nombre
                        : null,
                    Activo = p.Activo
                })
                .ToListAsync();

            return Ok(productos);
        }

        // ----------------------------------------------------
        // OBTENER PRODUCTO POR ID
        // GET: api/productos/1
        // Admin y Empleado
        // ----------------------------------------------------
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductoResponseDto>> GetProducto(int id)
        {
            var producto = await _context.Productos
                .AsNoTracking()
                .Where(p => p.IdProducto == id)
                .Select(p => new ProductoResponseDto
                {
                    IdProducto = p.IdProducto,
                    Nombre = p.Nombre,
                    Marca = p.Marca,
                    Variante = p.Variante,
                    PrecioCompra = p.PrecioCompra,
                    PrecioVenta = p.PrecioVenta,
                    Stock = p.Stock,
                    Imagen = p.Imagen,
                    IdCategoria = p.IdCategoria,
                    CategoriaNombre = p.Categoria != null
                        ? p.Categoria.Nombre
                        : null,
                    Activo = p.Activo
                })
                .FirstOrDefaultAsync();

            if (producto == null)
            {
                return NotFound(new
                {
                    mensaje = "Producto no encontrado."
                });
            }

            return Ok(producto);
     
        }

        // ----------------------------------------------------
        // CREAR PRODUCTO
        // POST: api/productos
        // Admin y Empleado
        // ----------------------------------------------------
        [HttpPost]
        public async Task<ActionResult<ProductoResponseDto>> CrearProducto(
            CrearProductoDto dto)
        {
            // Verificamos que la categoría exista.
            bool categoriaExiste = await _context.Categorias
                .AnyAsync(c => c.IdCategoria == dto.IdCategoria);

            if (!categoriaExiste)
            {
                return BadRequest(new
                {
                    mensaje = "La categoría indicada no existe."
                });
            }

            // Creamos el producto.
            var producto = new Models.Producto
            {
                Nombre = dto.Nombre,
                Marca = dto.Marca,
                Variante = dto.Variante,
                PrecioCompra = dto.PrecioCompra,
                PrecioVenta = dto.PrecioVenta,

                // El stock siempre empieza en cero.
                Stock = 0,

                // La imagen se cargará después mediante otro endpoint.
                Imagen = null,

                IdCategoria = dto.IdCategoria,
                Activo = true
            };

            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();

            // Buscamos el nombre de la categoría para devolverlo.
            string? categoriaNombre = await _context.Categorias
                .Where(c => c.IdCategoria == producto.IdCategoria)
                .Select(c => c.Nombre)
                .FirstOrDefaultAsync();

            var respuesta = new ProductoResponseDto
            {
                IdProducto = producto.IdProducto,
                Nombre = producto.Nombre,
                Marca = producto.Marca,
                Variante = producto.Variante,
                PrecioCompra = producto.PrecioCompra,
                PrecioVenta = producto.PrecioVenta,
                Stock = producto.Stock,
                Imagen = producto.Imagen,
                IdCategoria = producto.IdCategoria,
                CategoriaNombre = categoriaNombre,
                Activo = producto.Activo
            };

            return CreatedAtAction(
                nameof(GetProducto),
                new { id = producto.IdProducto },
                respuesta
            );
        }

        // ----------------------------------------------------
        // EDITAR PRODUCTO
        // PUT: api/productos/{id}
        // Admin y Empleado
        // ----------------------------------------------------
        [HttpPut("{id}")]
        public async Task<IActionResult> EditarProducto(
            int id,
            EditarProductoDto dto)
        {
            // Buscamos el producto.
            var producto = await _context.Productos
                .FirstOrDefaultAsync(p => p.IdProducto == id);

            if (producto == null)
            {
                return NotFound(new
                {
                    mensaje = "Producto no encontrado."
                });
            }

            // Verificamos que la categoría exista.
            bool categoriaExiste = await _context.Categorias
                .AnyAsync(c => c.IdCategoria == dto.IdCategoria);

            if (!categoriaExiste)
            {
                return BadRequest(new
                {
                    mensaje = "La categoría indicada no existe."
                });
            }

            // Modificamos solamente los datos permitidos.
            producto.Nombre = dto.Nombre;
            producto.Marca = dto.Marca;
            producto.Variante = dto.Variante;
            producto.PrecioCompra = dto.PrecioCompra;
            producto.PrecioVenta = dto.PrecioVenta;
            producto.IdCategoria = dto.IdCategoria;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Producto actualizado correctamente.",
                idProducto = producto.IdProducto
            });
        }

        // ----------------------------------------------------
        // DAR DE BAJA PRODUCTO
        // DELETE: api/productos/{id}
        // Solo Admin
        // ----------------------------------------------------
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarProducto(int id)
        {
            var producto = await _context.Productos
                .FirstOrDefaultAsync(p => p.IdProducto == id);

            if (producto == null)
            {
                return NotFound(new
                {
                    mensaje = "Producto no encontrado."
                });
            }

            if (!producto.Activo)
            {
                return BadRequest(new
                {
                    mensaje = "El producto ya se encuentra dado de baja."
                });
            }

            // Baja lógica: no eliminamos físicamente el registro.
            producto.Activo = false;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Producto dado de baja correctamente.",
                idProducto = producto.IdProducto
            });
        }
    }

}
