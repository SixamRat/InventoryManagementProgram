using InventorySystem.API.Data;
using InventorySystem.API.DTOs;
using InventorySystem.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeightReadingsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public WeightReadingsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<WeightReadingResultDto>> PostWeight(
            CreateWeightReadingDto dto)
        {
            var scale = await _context.Scales
                .Include(s => s.Product)
                .FirstOrDefaultAsync(s => s.SerialNumber == dto.SerialNumber);

            if (scale == null)
                return NotFound("Ingen våg med det serienumret hittades.");

            // Spara viktavläsning
            var reading = new WeightReading
            {
                WeightInKg = dto.WeightInKg,
                Timestamp = DateTime.UtcNow,
                ScaleId = scale.Id
            };

            _context.WeightReadings.Add(reading);
            await _context.SaveChangesAsync();

            // Räkna om vikten till rätt enhet med ConversionFactor
            double convertedValue = dto.WeightInKg;
            string unit = "Kg";
            string productName = "Okänd";

            if (scale.Product != null)
            {
                convertedValue = dto.WeightInKg * scale.Product.ConversionFactor;
                unit = scale.Product.Unit;
                productName = scale.Product.Name;
            }

            var result = new WeightReadingResultDto
            {
                ScaleId = scale.Id,
                SerialNumber = scale.SerialNumber,
                WeightInKg = dto.WeightInKg,
                ConvertedValue = Math.Round(convertedValue, 2),
                Unit = unit,
                ProductName = productName,
                Timestamp = reading.Timestamp
            };

            return Ok(result);
        }

        [HttpGet("latest/{scaleId}")]
        public async Task<ActionResult<WeightReadingResultDto>> GetLatest(int scaleId)
        {
            var scale = await _context.Scales
                .Include(s => s.Product)
                .FirstOrDefaultAsync(s => s.Id == scaleId);

            if (scale == null)
                return NotFound("Vågen finns inte.");

            var latest = await _context.WeightReadings
                .Where(w => w.ScaleId == scaleId)
                .OrderByDescending(w => w.Timestamp)
                .FirstOrDefaultAsync();

            if (latest == null)
                return NotFound("Inga avläsningar finns för den vågen.");

            double convertedValue = latest.WeightInKg;
            string unit = "Kg";
            string productName = "Okänd";

            if (scale.Product != null)
            {
                convertedValue = latest.WeightInKg * scale.Product.ConversionFactor;
                unit = scale.Product.Unit;
                productName = scale.Product.Name;
            }

            return Ok(new WeightReadingResultDto
            {
                ScaleId = scale.Id,
                SerialNumber = scale.SerialNumber,
                WeightInKg = latest.WeightInKg,
                ConvertedValue = Math.Round(convertedValue, 2),
                Unit = unit,
                ProductName = productName,
                Timestamp = latest.Timestamp
            });
        }
    }
}