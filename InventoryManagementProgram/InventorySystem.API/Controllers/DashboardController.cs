using InventorySystem.API.Data;
using InventorySystem.API.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }        
        //Hämtar alla vågar med vikt och prduktinfo
        [HttpGet("scales")]
        public async Task<ActionResult<List<DashboardScaleDto>>> GetDashboardScales(
            [FromQuery] int? teamId = null)
        {
            var query = _context.Scales
                .Include(s => s.Team)
                .Include(s => s.Product)
                .Include(s => s.WeightReadings)
                .AsQueryable();

            // Filtrera på team 
            if (teamId.HasValue)
            {
                query = query.Where(s => s.TeamId == teamId.Value);
            }

            var scales = await query.ToListAsync();

            var result = scales.Select(s =>
            {
                var latest = s.WeightReadings
                    .OrderByDescending(w => w.Timestamp)
                    .FirstOrDefault();

                double? convertedValue = null;
                if (latest != null && s.Product != null)
                {
                    convertedValue = Math.Round(
                        latest.WeightInKg * s.Product.ConversionFactor, 2);
                }
                else if (latest != null)
                {
                    convertedValue = latest.WeightInKg;
                }

                return new DashboardScaleDto
                {
                    Id = s.Id,
                    SerialNumber = s.SerialNumber,
                    TeamId = s.TeamId,
                    TeamName = s.Team.Name,
                    ProductName = s.Product?.Name,
                    ProductUnit = s.Product?.Unit,
                    LatestWeightKg = latest?.WeightInKg,
                    ConvertedValue = convertedValue,
                    LastReading = latest?.Timestamp
                };
            }).ToList();

            return Ok(result);
        }
    }
}