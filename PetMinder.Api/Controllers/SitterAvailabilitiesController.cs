using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetMinder.Models;
using PetMinder.Shared.DTO;
using WebApplication1.Services.Interfaces;

namespace WebApplication1.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SitterAvailabilitiesController : ControllerBase
{
    private readonly ISitterAvailabilityService _sitterAvailabilityService;

    public SitterAvailabilitiesController(ISitterAvailabilityService sitterAvailabilityService)
    {
        _sitterAvailabilityService = sitterAvailabilityService;
    }

    private long GetUserId() => long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
    
    [HttpGet("me")]
    [Authorize(Roles = nameof(UserRole.Sitter))]
    public async Task<IActionResult> GetMySitterAvailabilities()
    {
        var sitterId = GetUserId();
        var availabilities = await _sitterAvailabilityService.GetSitterAvailabilitiesAsync(sitterId);
        return Ok(availabilities);
    }

    [HttpGet("{sitterId}")] 
    public async Task<IActionResult> GetSitterAvailabilities(long sitterId)
    {
        var availabilities = await _sitterAvailabilityService.GetSitterAvailabilitiesAsync(sitterId);
        if (availabilities == null || !availabilities.Any())
        {
            return NotFound("Sitter or availability not found.");
        }
        return Ok(availabilities);
    }

    [HttpPost]
    [Authorize(Roles = nameof(UserRole.Sitter))]
    public async Task<IActionResult> AddSitterAvailability([FromBody] AddSitterAvailabilityDTO dto)
    {
        var sitterId = GetUserId();
        try
        {
            var availability = await _sitterAvailabilityService.AddSitterAvailabilityAsync(sitterId, dto);
            return CreatedAtAction(nameof(GetMySitterAvailabilities), new { availabilityId = availability.AvailabilityId }, availability);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error adding availability.", details = ex.Message });
        }
    }
    
    [HttpPut("{availabilityId}")] 
    [Authorize(Roles = nameof(UserRole.Sitter))]
    public async Task<IActionResult> UpdateSitterAvailability(long availabilityId, [FromBody] UpdateSitterAvailabilityDTO dto)
    {
        var sitterId = GetUserId();
        try
        {
            var result = await _sitterAvailabilityService.UpdateSitterAvailabilityAsync(sitterId, availabilityId, dto);
            if (!result)
            {
                return NotFound("Availability slot not found or not owned by this sitter.");
            }
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message }); 
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error updating availability.", details = ex.Message });
        }
    }
    
    [HttpDelete("{availabilityId}")] 
    [Authorize(Roles = nameof(UserRole.Sitter))]
    public async Task<IActionResult> DeleteSitterAvailability(long availabilityId)
    {
        var sitterId = GetUserId();
        try
        {
            var result = await _sitterAvailabilityService.DeleteSitterAvailabilityAsync(sitterId, availabilityId);
            if (!result)
            {
                return NotFound("Availability slot not found or not owned by this sitter.");
            }
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error deleting availability.", details = ex.Message });
        }
    }
    
    [HttpGet("search")] 
    public async Task<ActionResult<List<UserProfileDTO>>> SearchAvailableSitters(
        [FromQuery] SitterSearchRequestDTO searchRequest)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var ownerId = GetUserId();
        
        try
        {
            var sitters = await _sitterAvailabilityService.FindAvailableSittersAsync(searchRequest, ownerId);
            if (sitters == null || !sitters.Any())
            {
                return NotFound("No sitters found for the specified availability.");
            }
            return Ok(sitters);
        }
        catch (ArgumentException ex) 
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while searching for sitters." });
        }
    }
}