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
    public class CategoriasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoriasController(AppDbContext context)
        {
            _context = context;
        }

        // ----------------------------------------------------
        // LISTAR TODAS LAS CATEGORÍAS
        // GET: api/categorias
        // Admin y Empleado
        // ----------------------------------------------------
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoriaResponseDto>>> GetCategorias()
        {
            var categorias = await _context.Categorias
                .AsNoTracking()
                .OrderBy(c => c.IdCategoria)
                .Select(c => new CategoriaResponseDto
                {
                    IdCategoria = c.IdCategoria,
                    Nombre = c.Nombre,
                    Descripcion = c.Descripcion
                })
                .ToListAsync();

            return Ok(categorias);
        }

        // ----------------------------------------------------
        // OBTENER CATEGORÍA POR ID
        // GET: api/categorias/1
        // Admin y Empleado
        // ----------------------------------------------------
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoriaResponseDto>> GetCategoria(int id)
        {
            var categoria = await _context.Categorias
                .AsNoTracking()
                .Where(c => c.IdCategoria == id)
                .Select(c => new CategoriaResponseDto
                {
                    IdCategoria = c.IdCategoria,
                    Nombre = c.Nombre,
                    Descripcion = c.Descripcion
                })
                .FirstOrDefaultAsync();

            if (categoria == null)
            {
                return NotFound(new
                {
                    mensaje = "Categoría no encontrada."
                });
            }

            return Ok(categoria);
        }

        // ----------------------------------------------------
        // CREAR CATEGORÍA
        // POST: api/categorias
        // Solo Admin
        // ----------------------------------------------------
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<CategoriaResponseDto>> CrearCategoria(
            CrearCategoriaDto dto)
        {
            var categoria = new Models.Categoria
            {
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion
            };

            _context.Categorias.Add(categoria);

            await _context.SaveChangesAsync();

            var respuesta = new CategoriaResponseDto
            {
                IdCategoria = categoria.IdCategoria,
                Nombre = categoria.Nombre,
                Descripcion = categoria.Descripcion
            };

            return CreatedAtAction(
                nameof(GetCategoria),
                new { id = categoria.IdCategoria },
                respuesta
            );
        }

        // ----------------------------------------------------
        // EDITAR CATEGORÍA
        // PUT: api/categorias/{id}
        // Solo Admin
        // ----------------------------------------------------
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditarCategoria(
            int id,
            EditarCategoriaDto dto)
        {
            var categoria = await _context.Categorias
                .FirstOrDefaultAsync(c => c.IdCategoria == id);

            if (categoria == null)
            {
                return NotFound(new
                {
                    mensaje = "Categoría no encontrada."
                });
            }

            categoria.Nombre = dto.Nombre;
            categoria.Descripcion = dto.Descripcion;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Categoría actualizada correctamente.",
                idCategoria = categoria.IdCategoria
            });
        }

        // ----------------------------------------------------
        // ELIMINAR CATEGORÍA
        // DELETE: api/categorias/{id}
        // Solo Admin
        // ----------------------------------------------------
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EliminarCategoria(int id)
        {
            var categoria = await _context.Categorias
                .FirstOrDefaultAsync(c => c.IdCategoria == id);

            if (categoria == null)
            {
                return NotFound(new
                {
                    mensaje = "Categoría no encontrada."
                });
            }

            // No permitimos eliminar una categoría que tenga productos asociados.
            bool tieneProductos = await _context.Productos
                .AnyAsync(p => p.IdCategoria == id);

            if (tieneProductos)
            {
                return BadRequest(new
                {
                    mensaje = "No se puede eliminar la categoría porque tiene productos asociados."
                });
            }

            _context.Categorias.Remove(categoria);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Categoría eliminada correctamente.",
                idCategoria = categoria.IdCategoria
            });
        }
    }
}