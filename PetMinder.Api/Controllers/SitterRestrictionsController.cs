using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetMinder.Models;
using PetMinder.Shared.DTO;
using WebApplication1.Services.Interfaces;

namespace WebApplication1.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = nameof(UserRole.Sitter))]
public class SitterRestrictionsController : ControllerBase
{
    private readonly IRestrictionService _restrictionService;

        public SitterRestrictionsController(IRestrictionService restrictionService)
        {
            _restrictionService = restrictionService;
        }

        private long GetUserId() => long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

        [HttpGet("me")]
        public async Task<IActionResult> GetMyRestrictions()
        {
            var sitterId = GetUserId();
            var restrictions = await _restrictionService.GetSitterRestrictionsAsync(sitterId);
            return Ok(restrictions);
        }
        
        [HttpGet("{sitterId}")]
        [Authorize]
        public async Task<IActionResult> GetSitterRestrictions(long sitterId)
        {
            var restrictions = await _restrictionService.GetSitterRestrictionsAsync(sitterId);
            if (restrictions == null || !restrictions.Any()) return NotFound("Sitter or restrictions not found.");
            return Ok(restrictions);
        }

        [HttpPost]
        public async Task<IActionResult> AddSitterRestriction([FromBody] AddSitterRestrictionDTO dto)
        {
            var sitterId = GetUserId();
            try
            {
                var restriction = await _restrictionService.AddSitterRestrictionAsync(sitterId, dto);
                return CreatedAtAction(nameof(GetMyRestrictions), restriction);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error adding restriction.", details = ex.Message });
            }
        }
        
        [HttpDelete("{restrictionTypeId}")]
        public async Task<IActionResult> RemoveSitterRestriction(long restrictionTypeId)
        {
            var sitterId = GetUserId();
            var result = await _restrictionService.RemoveSitterRestrictionAsync(sitterId, restrictionTypeId);
            if (!result) return NotFound("Restriction not found or not associated with this sitter.");
            return NoContent();
        }
}