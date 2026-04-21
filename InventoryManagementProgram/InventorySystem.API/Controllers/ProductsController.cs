using InventorySystem.API.Data;
using InventorySystem.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        
        /// Hämta produkten som är kopplad till en viss våg.
        
        [HttpGet("byscale/{scaleId}")]
        public async Task<ActionResult<Product>> GetProductByScale(int scaleId)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.ScaleId == scaleId);

            if (product == null) return NotFound();
            return Ok(product);
        }

       
        /// Skapa eller uppdatera produkten på en våg.
        
        [HttpPost("byscale/{scaleId}")]
        public async Task<ActionResult<Product>> SetProductOnScale(int scaleId, SetProductDto dto)
        {
            var scale = await _context.Scales.FindAsync(scaleId);
            if (scale == null) return NotFound("Vågen finns inte.");

            var existing = await _context.Products
                .FirstOrDefaultAsync(p => p.ScaleId == scaleId);

            if (existing != null)
            {
                existing.Name = dto.Name;
                existing.Unit = dto.Unit;
                existing.ConversionFactor = dto.ConversionFactor;
            }
            else
            {
                var product = new Product
                {
                    Name = dto.Name,
                    Unit = dto.Unit,
                    ConversionFactor = dto.ConversionFactor,
                    ScaleId = scaleId
                };
                _context.Products.Add(product);
            }

            await _context.SaveChangesAsync();

            var result = await _context.Products
                .FirstAsync(p => p.ScaleId == scaleId);

            return Ok(result);
        }

        
        /// Ta bort produkten från en våg.
        
        [HttpDelete("byscale/{scaleId}")]
        public async Task<IActionResult> RemoveProductFromScale(int scaleId)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.ScaleId == scaleId);

            if (product == null) return NotFound();

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }

    public class SetProductDto
    {
        public string Name { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public double ConversionFactor { get; set; } = 1.0;
    }
}