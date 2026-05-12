using InventorySystem.API.Data;
using InventorySystem.API.DTOs;
using InventorySystem.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "AllRoles")]
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
            var scale = await _context.Scales.FindAsync(dto.ScaleId);
            if (scale == null)
                return BadRequest("Vågen finns inte.");

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
        [Authorize(Policy = "ManagerOrAdmin")]
        public async Task<ActionResult> ReviewChangeRequest(
            int id, ReviewChangeRequestDto dto)
        {
            if (dto.Status != "Approved" && dto.Status != "Denied")
                return BadRequest("Status måste vara 'Approved' eller 'Denied'.");

            var changeRequest = await _context.ChangeRequ