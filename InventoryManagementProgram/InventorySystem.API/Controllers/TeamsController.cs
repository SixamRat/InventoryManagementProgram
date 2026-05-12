using InventorySystem.API.Data;
using InventorySystem.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "ManagerOrAdmin")]
    public class TeamsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TeamsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/teams
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Team>>> GetTeams()
        {
            return await _context.Teams
                .Include(t => t.Customer)
                .Include(t => t.Users)
                .ToListAsync();
        }

        // GET: api/teams/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Team>> GetTeam(int id)
        {
            var team = await _context.Teams
                .Include(t => t.Customer)
                .Include(t => t.Users)
                .Include(t => t.Scales)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (team == null) return NotFound();
            return team;
        }

        // POST: api/teams
        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<Team>> CreateTeam(Team team)
        {
            _context.Teams.Add(team);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetTeam), new { id = team.Id }, team);
        }

        // DELETE: api/teams/5
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteTeam(int id)
        {
            var team = await _context.Teams.FindAsync(id);
            if (team == null) return NotFound();

            _context.Teams.Remove(team);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}