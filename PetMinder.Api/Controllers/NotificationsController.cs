using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetMinder.Shared.DTO;
using WebApplication1.Services.Interfaces;

namespace PetMinder.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationsController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }
    
    private bool TryGetUserId(out long userId)
    {
        userId = 0;

        if (User?.Identity?.IsAuthenticated != true)
            return false;

        var claim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                    ?? User.FindFirst("sub")?.Value
                    ?? User.FindFirst("id")?.Value;

        if (string.IsNullOrEmpty(claim))
            return false;

        if (!long.TryParse(claim, out userId))
            return false;

        return true;
    }

    [HttpGet]
    public async Task<IActionResult> GetNotifications()
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var notifications = await _notificationService.GetUserNotificationsAsync(userId);
        return Ok(notifications);
    }

    [HttpGet("count")]
    public async Task<IActionResult> GetNotificationCount()
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var count = await _notificationService.GetNotificationCountAsync(userId);
        return Ok(new { count });
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> DeleteNotification(long id)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        await _notificationService.DeleteNotificationAsync(id, userId);
        return NoContent();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteAllNotifications()
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        await _notificationService.DeleteAllNotificationsAsync(userId);
        return NoContent();
    }

    [HttpGet("preferences")]
    public async Task<IActionResult> GetPreferences()
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var preferences = await _notificationService.GetPreferencesAsync(userId);
        return Ok(preferences);
    }

    [HttpPut("preferences")]
    public async Task<IActionResult> UpdatePreference([FromBody] NotificationPreferenceDTO preferenceDto)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        await _notificationService.UpdatePreferenceAsync(userId, preferenceDto.NotificationType, preferenceDto.IsEnabled);
        return NoContent();
    }
}
