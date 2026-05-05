using InventorySystem.API.Data;
using InventorySystem.API.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly AppDbContext _context;

        public InventoryController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<InventoryListDto>> GetInventory(
            [FromQuery] int? teamId = null)
        {
            var query = _context.Scales
                .Include(s => s.Team)
                .Include(s => s.Product)
                .Include(s => s.WeightReadings)
                .Where(s => s.Product != null)
                .AsQueryable();

            if (teamId.HasValue)
            {
                query = query.Where(s => s.TeamId == teamId.Value);
            }

            var scales = await query.ToListAsync();

            var items = scales
                .Where(s => s.Product != null)
                .Select(s =>
                {
                    var latest = s.WeightReadings
                        .OrderByDescending(w => w.Timestamp)
                        .FirstOrDefault();

                    return new
                    {
                        s.Product!.Name,
                        s.Product.Unit,
                        ConvertedValue = latest != null
                            ? latest.WeightInKg * s.Product.ConversionFactor
                            : 0
                    };
                })
                .GroupBy(x => new { x.Name, x.Unit })
                .Select(g => new InventoryItemDto
                {
                    ProductName = g.Key.Name,
                    Unit = g.Key.Unit,
                    TotalQuantity = Math.Round(g.Sum(x => x.ConvertedValue), 2),
                    NumberOfScales = g.Count()
                })
                .OrderBy(i => i.ProductName)
                .ToList();

            // Filtrera på teamnamn
            string? teamName = null;
            if (teamId.HasValue)
            {
                var team = await _context.Teams.FindAsync(teamId.Value);
                teamName = team?.Name;
            }

            var result = new InventoryListDto
            {
                TeamName = teamName,
                GeneratedAt = DateTime.UtcNow,
                Items = items
            };

            return Ok(result);
        }
    }
}