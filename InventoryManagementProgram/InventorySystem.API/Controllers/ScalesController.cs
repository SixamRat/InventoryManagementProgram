using InventorySystem.API.Data;
using InventorySystem.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace InventorySystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "AllRoles")]
    public class ScalesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ScalesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/scales
        // Admin ser alla vågar, Personal/Kökschef ser bara sitt teams vågar
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Scale>>> GetScales()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value
                         ?? User.FindFirst("preferred_username")?.Value;

            var userRoles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();

            // Admin ser allt
            if (userRoles.Contains("Admin"))
            {
                return await _context.Scales
                    .Include(s => s.Team)
                    .Include(s => s.Product)
                    .Include(s => s.WeightReadings.OrderByDescending(w => w.Timestamp).Take(1))
                    .ToListAsync();
            }

            // Hitta användarens team via e-post
            var dbUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == userEmail);

            if (dbUser == null)
                return Forbid();

            // Personal och Kökschef ser bara sitt teams vågar
            return await _context.Scales
                .Where(s => s.TeamId == dbUser.TeamId)
                .Include(s => s.Team)
                .Include(s => s.Product)
                .Include(s => s.WeightReadings.OrderByDescending(w => w.Timestamp).Take(1))
                .ToListAsync();
        }

        // GET: api/scales/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Scale>> GetScale(int id)
        {
            var scale = await _context.Scales
                .Include(s => s.Team)
                .Include(s => s.Product)
                .Include(s => s.WeightReadings)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (scale == null) return NotFound();
            return scale;
        }

        // POST: api/scales
        [HttpPost]
        [Authorize(Policy = "ManagerOrAdmin")]
        public async Task<ActionResult<Scale>> CreateScale(Scale scale)
        {
            _context.Scales.Add(scale);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetScale), new { id = scale.Id }, scale);
        }

        // DELETE: api/scales/5
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
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