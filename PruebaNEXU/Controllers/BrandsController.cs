using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PruebaNEXU.DataContext;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PruebaNEXU.Controllers
{
    [ApiController]
    //[Route("brands")]
    public class BrandsController : ControllerBase
    {
        private readonly PRUEBA_NEXUContext _context;

        public BrandsController(PRUEBA_NEXUContext context)
        {
            _context = context;
        }

        // GET /brands
        [HttpGet("brands")]
        public async Task<IActionResult> GetBrands()
        {
            var brands = await _context.Brands.ToListAsync();

            return Ok(brands);
        }

        [HttpGet("{id}/models")]
        public async Task<IActionResult> GetModelsByBrand(int id)
        {
            var brandExists = await _context.Brands.AnyAsync(b => b.Id == id);
            if (!brandExists)
                return NotFound("Brand no encontrada");

            var models = await _context.Models
                .Where(m => m.BrandNameNavigation.Id == id)
                .AsNoTracking()
                .ToListAsync();

            return Ok(models);
        }

        [HttpPost ("brands")]
        public async Task<IActionResult> CreateBrand([FromBody] Brand brand)
        {
            if (string.IsNullOrWhiteSpace(brand.Nombre))
                return BadRequest("El nombre de la marca es obligatorio");
            var exists = await _context.Brands
            .AnyAsync(b => b.Nombre == brand.Nombre);

            if (exists)
                return BadRequest("La marca ya existe");
            _context.Brands.Add(brand);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBrands), new { id = brand.Id }, brand);
        }

        [HttpPost("brands/{id}/models")]
        public async Task<IActionResult> CreateModel(int id, [FromBody] Model mod)
        {
            if (mod == null)
                return BadRequest("El body es requerido");

            if (string.IsNullOrWhiteSpace(mod.Name))
                return BadRequest("El nombre del modelo es obligatorio");
            // Verificar que la Brand exista
            var brand = await _context.Brands
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == id);
            if (brand == null)
                return NotFound("Brand no encontrada");
            // Validar duplicados por brand
            var modelExists = await _context.Models
                .AnyAsync(m =>
                    m.BrandName == brand.Nombre &&
                    m.Name.ToLower() == mod.Name.ToLower()
                );

            if (modelExists)
                return Conflict("El modelo ya existe para esta brand");

            // Validar AveragePrice (opcional)
            if (mod.AveragePrice.HasValue && mod.AveragePrice <= 100000)
                return BadRequest("El average price debe ser mayor a 100,000");

            var model = new Model
            {
                Name = mod.Name,
                AveragePrice = mod.AveragePrice,
                BrandName = brand.Nombre
            };

            _context.Models.Add(model);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetModelsByBrand),
                new { id = id },
                model
            );
        }

        [HttpPut("/models/{id}")]
        public async Task<IActionResult> UpdateModelPrice(int id,[FromBody] Model dto)
        {
            if (dto == null)
                return BadRequest("El body es requerido");

            if (!dto.AveragePrice.HasValue)
                return BadRequest("El average price es requerido");

            if (dto.AveragePrice <= 100000)
                return BadRequest("El average price debe ser mayor a 100,000");

            var model = await _context.Models.FindAsync(id);

            if (model == null)
                return NotFound($"El modelo con id {id} no existe");

            model.AveragePrice = dto.AveragePrice;

            await _context.SaveChangesAsync();

            return Ok(model);
        }

        [HttpGet("/models")]
        public async Task<IActionResult> GetModels([FromQuery] int? greater, [FromQuery] int? lower)
        {
            var query = _context.Models
                .AsNoTracking()
                .AsQueryable();

            if (greater.HasValue)
            {
                query = query.Where(m =>
                    m.AveragePrice.HasValue &&
                    m.AveragePrice > greater.Value);
            }

            if (lower.HasValue)
            {
                query = query.Where(m =>
                    m.AveragePrice.HasValue &&
                    m.AveragePrice < lower.Value);
            }

            var models = await query.ToListAsync();

            return Ok(models);
        }
    }
}
