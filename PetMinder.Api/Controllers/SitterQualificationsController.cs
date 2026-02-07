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
public class SitterQualificationsController : ControllerBase
{
    private readonly IQualificationService _qualificationService;

        public SitterQualificationsController(IQualificationService qualificationService)
        {
            _qualificationService = qualificationService;
        }

        private long GetUserId() => long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

        [HttpGet("me")]
        public async Task<IActionResult> GetMyQualifications()
        {
            var sitterId = GetUserId();
            var qualifications = await _qualificationService.GetSitterQualificationsAsync(sitterId);
            return Ok(qualifications);
        }
        
        [HttpGet("{sitterId}")]
        [Authorize]
        public async Task<IActionResult> GetSitterQualifications(long sitterId)
        {
            var qualifications = await _qualificationService.GetSitterQualificationsAsync(sitterId);
            if (qualifications == null || !qualifications.Any()) return NotFound("Sitter or qualifications not found.");
            return Ok(qualifications);
        }

        [HttpPost]
        public async Task<IActionResult> AddSitterQualification([FromBody] AddSitterQualificationDTO dto)
        {
            var sitterId = GetUserId();
            try
            {
                var qualification = await _qualificationService.AddSitterQualificationAsync(sitterId, dto);
                return CreatedAtAction(nameof(GetMyQualifications), qualification); 
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
                return StatusCode(500, new { message = "Error adding qualification.", details = ex.Message });
            }
        }

        [HttpDelete("{qualificationTypeId}")]
        public async Task<IActionResult> RemoveSitterQualification(long qualificationTypeId)
        {
            var sitterId = GetUserId();
            var result = await _qualificationService.RemoveSitterQualificationAsync(sitterId, qualificationTypeId);
            if (!result) return NotFound("Qualification not found or not associated with this sitter.");
            return NoContent();
        }
}