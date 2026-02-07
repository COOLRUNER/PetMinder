using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetMinder.Data;
using PetMinder.Shared.DTO;
using WebApplication1.Services.Interfaces;

namespace WebApplication1.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AddressController : ControllerBase
{
    private readonly IAddressService _addressService;
    private readonly ILogger<AddressController> _logger;
    
    public AddressController(IAddressService addressService, ILogger<AddressController> logger)
    {
        _addressService = addressService;
        _logger = logger;
    }
    
    private long GetUserId() => long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
    
    [HttpPost]
    public async Task<IActionResult> AddAddress([FromBody] CreateAddressDTO dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userId = GetUserId();
        try
        {
            var addressDto = await _addressService.AddAddressAsync(userId, dto);
            return CreatedAtAction(nameof(GetUserAddresses), new { id = addressDto.UserAddressId }, addressDto);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Failed to add address for user {UserId}", userId);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding address for user {UserId}", userId);
            return StatusCode(500, new { message = "An error occurred while adding the address." });
        }
    }
    
    [HttpGet("me")]
    public async Task<ActionResult<List<AddressDTO>>> GetUserAddresses()
    {
        var userId = GetUserId();
        var addresses = await _addressService.GetUserAddressesAsync(userId);
        return Ok(addresses);
    }
    
    [HttpDelete("{userAddressId}")]
    public async Task<IActionResult> DeleteUserAddress(long userAddressId)
    {
        var userId = GetUserId();
        var success = await _addressService.DeleteUserAddressAsync(userId, userAddressId);

        if (!success)
        {
            return NotFound(new { message = "Address not found or you do not have permission to delete it." });
        }

        return NoContent();
    }
}