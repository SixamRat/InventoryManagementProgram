using InventorySystem.API.Data;
using InventorySystem.API.DTOs;
using InventorySystem.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChangeRequestsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ChangeRequestsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<ChangeRequestDto>>> GetChangeRequests(
            [FromQuery] string? status = null)
        {
            var query = _context.ChangeRequests
                .Include(c => c.Scale)
                .Include(c => c.CreatedBy)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(c => c.Status == status);
            }

            var result = await query.Select(c => new ChangeRequestDto
            {
                Id = c.Id,
                ProposedProduct = c.ProposedProduct,
                ProposedUnit = c.ProposedUnit,
                Status = c.Status,
                ScaleId = c.ScaleId,
                ScaleSerialNumber = c.Scale.SerialNumber,
                CreatedByUserId = c.CreatedByUserId,
                CreatedByUserName = c.CreatedBy.Name
            }).ToListAsync();

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<ChangeRequestDto>> CreateChangeRequest(
            CreateChangeRequestDto dto)
        {
            // Kolla att vågen finns
            var scale = await _context.Scales.FindAsync(dto.ScaleId);
            if (scale == null)
                return BadRequest("Vågen finns inte.");

            // Kolla att användaren finns
            var user = await _context.Users.FindAsync(dto.CreatedByUserId);
            if (user == null)
                return BadRequest("Användaren finns inte.");

            var changeRequest = new ChangeRequest
            {
                ProposedProduct = dto.ProposedProduct,
                ProposedUnit = dto.ProposedUnit,
                Status = "Pending",
                ScaleId = dto.ScaleId,
                CreatedByUserId = dto.CreatedByUserId
            };

            _context.ChangeRequests.Add(changeRequest);
            await _context.SaveChangesAsync();

            var result = new ChangeRequestDto
            {
                Id = changeRequest.Id,
                ProposedProduct = changeRequest.ProposedProduct,
                ProposedUnit = changeRequest.ProposedUnit,
                Status = changeRequest.Status,
                ScaleId = changeRequest.ScaleId,
                ScaleSerialNumber = scale.SerialNumber,
                CreatedByUserId = changeRequest.CreatedByUserId,
                CreatedByUserName = user.Name
            };

            return CreatedAtAction(nameof(GetChangeRequests), result);
        }

        [HttpPut("{id}/review")]
        public async Task<ActionResult> ReviewChangeRequest(
            int id, ReviewChangeRequestDto dto)
        {
            // Bara "Approved" och "Denied" är giltiga
            if (dto.Status != "Approved" && dto.Status != "Denied")
                return BadRequest("Status måste vara 'Approved' eller 'Denied'.");

            var changeRequest = await _context.ChangeRequests
                .Include(c => c.Scale)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (changeRequest == null)
                return NotFound("Ändringsförslaget finns inte.");

            if (changeRequest.Status != "Pending")
                return BadRequest("Förslaget har redan hanterats.");

            changeRequest.Status = dto.Status;

            // Om chefen godkänner: uppdatera produkten på vågen
            if (dto.Status == "Approved")
            {
                var existingProduct = await _context.Products
                    .FirstOrDefaultAsync(p => p.ScaleId == changeRequest.ScaleId);

                if (existingProduct != null)
                {
                    // Uppdatera befintlig produkt
                    existingProduct.Name = changeRequest.ProposedProduct;
                    existingProduct.Unit = changeRequest.ProposedUnit;
                }
                else
                {
                    // Skapa ny produkt på vågen
                    var product = new Product
                    {
                        Name = changeRequest.ProposedProduct,
                        Unit = changeRequest.ProposedUnit,
                        ConversionFactor = 1.0,
                        ScaleId = changeRequest.ScaleId
                    };
                    _context.Products.Add(product);
                }
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}