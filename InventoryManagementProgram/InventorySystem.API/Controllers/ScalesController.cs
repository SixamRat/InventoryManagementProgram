using InventorySystem.API.Data;
using InventorySystem.API.DTOs;
using InventorySystem.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace InventorySystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
   // [Authorize(Policy = "AllRoles")]
    public class ScalesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ScalesController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Hämtar alla vågar. Admin ser alla, övriga ser bara sitt teams vågar.
        /// </summary>
        //[HttpGet]
        //public async Task<ActionResult<List<ScaleDto>>> GetScales()
        //{
        //  var userEmail = User.FindFirst(ClaimTypes.Email)?.Value
        //             ?? User.FindFirst("preferred_username")?.Value;

        //var userRoles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();

        //IQueryable<Scale> query = _context.Scales
        //  .Include(s => s.Team)
        //.Include(s => s.Product);

        // Admin ser allt, övriga ser bara sitt teams vågar
        //if (!userRoles.Contains("Admin"))
        //{
        //  var dbUser = await _context.Users
        //    .FirstOrDefaultAsync(u => u.Email == userEmail);

        //if (dbUser == null)
        //  return Forbid();

        //query = query.Where(s => s.TeamId == dbUser.TeamId);
        //}

        //var scales = await query.Select(s => new ScaleDto
        //{
        //  Id = s.Id,
        //SerialNumber = s.SerialNumber,
        //QrCode = s.QrCode,
        //TeamId = s.TeamId,
        //TeamName = s.Team.Name,
        //ProductName = s.Product != null ? s.Product.Name : null,
        //ProductUnit = s.Product != null ? s.Product.Unit : null
        //}).ToListAsync();

        //return Ok(scales);
        //}

        ///------------For TESTING IN SWAGGER
        ///
        /// <summary>
        /// Hämtar alla vågar. Admin ser alla, övriga ser bara sitt teams vågar.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<ScaleDto>>> GetScales()
        {
            // TODO: Ta tillbaka rollkontroll efter testning
            var scales = await _context.Scales
                .Include(s => s.Team)
                .Include(s => s.Product)
                .Select(s => new ScaleDto
                {
                    Id = s.Id,
                    SerialNumber = s.SerialNumber,
                    QrCode = s.QrCode,
                    TeamId = s.TeamId,
                    TeamName = s.Team.Name,
                    ProductName = s.Product != null ? s.Product.Name : null,
                    ProductUnit = s.Product != null ? s.Product.Unit : null
                }).ToListAsync();

            return Ok(scales);
        }

        /// <summary>
        /// Hämtar en specifik våg med ID.
        /// </summary>
        /// <param name="id">Vågens ID</param>
        [HttpGet("{id}")]
        public async Task<ActionResult<ScaleDto>> GetScale(int id)
        {
            var scale = await _context.Scales
                .Include(s => s.Team)
                .Include(s => s.Product)
                .Where(s => s.Id == id)
                .Select(s => new ScaleDto
                {
                    Id = s.Id,
                    SerialNumber = s.SerialNumber,
                    QrCode = s.QrCode,
                    TeamId = s.TeamId,
                    TeamName = s.Team.Name,
                    ProductName = s.Product != null ? s.Product.Name : null,
                    ProductUnit = s.Product != null ? s.Product.Unit : null
                })
                .FirstOrDefaultAsync();

            if (scale == null) return NotFound();
            return Ok(scale);
        }

        /// <summary>
        /// Registrerar en ny våg. Kräver rollen Kökschef eller Admin.
        /// </summary>
        /// <param name="dto">Serienummer och team-ID</param>
        [HttpPost]
        //[Authorize(Policy = "ManagerOrAdmin")]
        public async Task<ActionResult<ScaleDto>> CreateScale(CreateScaleDto dto)
        {
            // Kolla att teamet finns
            var team = await _context.Teams.FindAsync(dto.TeamId);
            if (team == null)
                return BadRequest("Teamet finns inte.");

            // Kolla att serienumret inte redan används
            var exists = await _context.Scales
                .AnyAsync(s => s.SerialNumber == dto.SerialNumber);
            if (exists)
                return Conflict("En våg med det serienumret finns redan.");

            var scale = new Scale
            {
                SerialNumber = dto.SerialNumber,
                QrCode = Guid.NewGuid().ToString(),
                TeamId = dto.TeamId
            };

            _context.Scales.Add(scale);
            await _context.SaveChangesAsync();

            var result = new ScaleDto
            {
                Id = scale.Id,
                SerialNumber = scale.SerialNumber,
                QrCode = scale.QrCode,
                TeamId = scale.TeamId,
                TeamName = team.Name,
                ProductName = null,
                ProductUnit = null
            };

            return CreatedAtAction(nameof(GetScale), new { id = scale.Id }, result);
        }

        /// <summary>
        /// Uppdaterar en vågs serienummer och/eller team-tillhörighet.
        /// </summary>
        /// <param name="id">Vågens ID</param>
        /// <param name="dto">Nya värden</param>
        [HttpPut("{id}")]
        //[Authorize(Policy = "ManagerOrAdmin")]
        public async Task<ActionResult> UpdateScale(int id, UpdateScaleDto dto)
        {
            var scale = await _context.Scales.FindAsync(id);
            if (scale == null) return NotFound();

            // Kolla att teamet finns
            var team = await _context.Teams.FindAsync(dto.TeamId);
            if (team == null)
                return BadRequest("Teamet finns inte.");

            // Kolla att serienumret inte redan används av en annan våg
            var duplicate = await _context.Scales
                .AnyAsync(s => s.SerialNumber == dto.SerialNumber && s.Id != id);
            if (duplicate)
                return Conflict("En annan våg har redan det serienumret.");

            scale.SerialNumber = dto.SerialNumber;
            scale.TeamId = dto.TeamId;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Tar bort en våg. Kräver Admin-roll.
        /// </summary>
        /// <param name="id">Vågens ID</param>
        [HttpDelete("{id}")]
        //[Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteScale(int id)
        {
            var scale = await _context.Scales.FindAsync(id);
            if (scale == null) return NotFound();

            _context.Scales.Remove(scale);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}