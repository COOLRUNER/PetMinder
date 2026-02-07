using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using PetMinder.Shared.DTO;
using WebApplication1.Services.Interfaces;

namespace WebApplication1.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }
    
    [HttpGet("users")]
    public async Task<IActionResult> GetUsers([FromQuery] string? search)
    {
        var users = await _adminService.GetUsersAsync(search);
        return Ok(users);
    }

    [HttpPost("users/{userId}/flag")]
    public async Task<IActionResult> ToggleFlagUser(long userId)
    {
        var success = await _adminService.ToggleFlagUserAsync(userId);
        if (!success) return NotFound("User not found.");
            
        return Ok(new { Message = "User flag status updated." });
    }

    [HttpGet("users/{userId}")]
    public async Task<IActionResult> GetUserDetails(long userId)
    {
        var details = await _adminService.GetUserDetailsAsync(userId);
        if (details == null) return NotFound("User not found");
    
        return Ok(details);
    }
    
    [HttpGet("reviews/reported")]
    public async Task<IActionResult> GetReportedReviews()
    {
        var result = await _adminService.GetReportedReviewsAsync();
        return Ok(result);
    }

    [HttpDelete("reviews/{reviewId}")]
    public async Task<IActionResult> DeleteReview(long reviewId)
    {
        var success = await _adminService.DeleteReviewAsync(reviewId);
        if (!success) return NotFound();
        return Ok(new { Message = "Review deleted." });
    }

    [HttpDelete("reviews/{reviewId}/reports")]
    public async Task<IActionResult> DismissReviewReports(long reviewId)
    {
        var success = await _adminService.DismissReviewReportsAsync(reviewId);
        if (!success) return NotFound();
        return Ok(new { Message = "Reports dismissed. Review kept." });
    }
    
    [HttpGet("users/{userId}/conversations")]
    public async Task<IActionResult> GetUserConversations(long userId)
    {
        var conversations = await _adminService.GetUserConversationsAsync(userId);
        return Ok(conversations);
    }

    [HttpGet("conversations/{conversationId}/messages")]
    public async Task<IActionResult> GetConversationMessages(long conversationId)
    {
        var messages = await _adminService.GetConversationMessagesAsync(conversationId);
        return Ok(messages);
    }
    
    [HttpGet("points/policies")]
    public async Task<IActionResult> GetPolicies()
    {
        var policies = await _adminService.GetPointPoliciesAsync();
        return Ok(policies);
    }
    
    [HttpPut("points/policies")]
    public async Task<IActionResult> UpdatePolicy([FromBody] PointPolicyDTO dto)
    {
        var success = await _adminService.UpdatePointPolicyAsync(dto);
        if (!success) return NotFound(new { message = "Policy not found." });
        return Ok(new { message = "Policy updated successfully." });
    }
    
    [HttpPost("points/refund")]
    public async Task<IActionResult> ProcessRefund([FromBody] AdminTransactionAdjustmentDTO dto)
    {
        try
        {
            await _adminService.ProcessRefundAsync(dto);
            return Ok(new { message = "Refund processed successfully." });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An internal error occurred while processing the refund." });
        }
    }
    
    [HttpGet("users/reported")]
    public async Task<IActionResult> GetReportedUsers()
    {
        return Ok(await _adminService.GetReportedUsersAsync());
    }

    [HttpPost("users/{userId}/ban")]
    public async Task<IActionResult> BanUser(long userId)
    {
        var success = await _adminService.BanUserAsync(userId);
        return success ? Ok() : NotFound();
    }

    [HttpDelete("users/{userId}/reports")]
    public async Task<IActionResult> DismissUserReports(long userId)
    {
        var success = await _adminService.DismissUserReportsAsync(userId);
        return success ? Ok() : NotFound();
    }
    
    [HttpDelete("reports/{reportId}")]
    public async Task<IActionResult> DismissSingleReport(long reportId)
    {
        var success = await _adminService.DismissSingleUserReportAsync(reportId);
        return success ? Ok() : NotFound();
    }
}