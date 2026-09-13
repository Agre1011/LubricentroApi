using LubricentroApi.Data;
using LubricentroApi.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

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
        // LISTAR PRODUCTOS CON PAGINACIÓN
        // GET: api/productos?pagina=1&tamanoPagina=5
        // Admin y Empleado
        // ----------------------------------------------------
        [HttpGet]
        public async Task<ActionResult<RespuestaPaginadaDto<ProductoResponseDto>>> GetProductos(
            [FromQuery] ParametrosPaginacionDto parametros)
        {
            // Consulta base.
            var consulta = _context.Productos
                .AsNoTracking()
                .OrderBy(p => p.IdProducto);

            // Cantidad total de productos.
            int totalRegistros = await consulta.CountAsync();

            // Cantidad total de páginas.
            int totalPaginas = (int)Math.Ceiling(
                totalRegistros / (double)parametros.TamanoPagina
            );

            // Aplicamos la paginación.
            var productos = await consulta
                .Skip((parametros.Pagina - 1) * parametros.TamanoPagina)
                .Take(parametros.TamanoPagina)
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

            // Armamos la respuesta paginada.
            var respuesta = new RespuestaPaginadaDto<ProductoResponseDto>
            {
                PaginaActual = parametros.Pagina,
                TamanoPagina = parametros.TamanoPagina,
                TotalRegistros = totalRegistros,
                TotalPaginas = totalPaginas,
                TienePaginaAnterior = parametros.Pagina > 1,
                TienePaginaSiguiente = parametros.Pagina < totalPaginas,
                Datos = productos
            };

            return Ok(respuesta);
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

        // ----------------------------------------------------
        // SUBIR O REEMPLAZAR IMAGEN DE PRODUCTO
        // POST: api/productos/{id}/imagen
        // Admin y Empleado
        // ----------------------------------------------------
        [HttpPost("{id}/imagen")]
        public async Task<IActionResult> SubirImagen(
            int id,
            IFormFile archivo)
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
                    mensaje = "No se puede cargar una imagen a un producto dado de baja."
                });
            }

            // Verificar que realmente se haya enviado un archivo.
            if (archivo == null || archivo.Length == 0)
            {
                return BadRequest(new
                {
                    mensaje = "Debe seleccionar una imagen."
                });
            }

            // Tamaño máximo: 5 MB.
            const long tamanoMaximo = 5 * 1024 * 1024;

            if (archivo.Length > tamanoMaximo)
            {
                return BadRequest(new
                {
                    mensaje = "La imagen no puede superar los 5 MB."
                });
            }

            // Formatos permitidos.
            var extensionesPermitidas = new[]
            {
        ".jpg",
        ".jpeg",
        ".png"
    };

            var extension = Path.GetExtension(archivo.FileName)
                .ToLowerInvariant();

            if (!extensionesPermitidas.Contains(extension))
            {
                return BadRequest(new
                {
                    mensaje = "Formato no permitido. Solo se aceptan JPG, JPEG o PNG."
                });
            }

            // También comprobamos el tipo MIME.
            var tiposPermitidos = new[]
            {
        "image/jpeg",
        "image/png"
    };

            if (!tiposPermitidos.Contains(archivo.ContentType.ToLowerInvariant()))
            {
                return BadRequest(new
                {
                    mensaje = "El archivo seleccionado no es una imagen válida."
                });
            }

            // Ruta física donde se guardarán las imágenes.
            var carpetaUploads = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "uploads"
            );

            // Por seguridad, la creamos si no existiera.
            Directory.CreateDirectory(carpetaUploads);

            // Si el producto ya tenía una imagen, borramos la anterior.
            if (!string.IsNullOrWhiteSpace(producto.Imagen))
            {
                var nombreImagenAnterior = Path.GetFileName(producto.Imagen);

                var rutaAnterior = Path.Combine(
                    carpetaUploads,
                    nombreImagenAnterior
                );

                if (System.IO.File.Exists(rutaAnterior))
                {
                    System.IO.File.Delete(rutaAnterior);
                }
            }

            // Creamos un nombre único para evitar archivos repetidos.
            var nombreArchivo = $"{Guid.NewGuid()}{extension}";

            var rutaArchivo = Path.Combine(
                carpetaUploads,
                nombreArchivo
            );

            // Guardar físicamente la imagen.
            await using (var stream = new FileStream(
                rutaArchivo,
                FileMode.Create))
            {
                await archivo.CopyToAsync(stream);
            }

            // Guardamos únicamente la ruta relativa en SQL Server.
            producto.Imagen = $"/uploads/{nombreArchivo}";

            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Imagen cargada correctamente.",
                idProducto = producto.IdProducto,
                imagen = producto.Imagen
            });
        }
    }

}
