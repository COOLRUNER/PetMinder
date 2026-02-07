using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetMinder.Shared.DTO;
using WebApplication1.Services.Interfaces;

namespace WebApplication1.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReviewsController : ControllerBase
{
    private readonly IReviewService _reviewService;
    private readonly IReportService _reportService;
    private readonly ILogger<ReviewsController> _logger;

    public ReviewsController(IReviewService reviewService, IReportService reportService, ILogger<ReviewsController> logger)
    {
        _reviewService = reviewService;
        _reportService = reportService;
        _logger = logger;
    }

    private long GetUserId() => long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

    [HttpPost]
    public async Task<IActionResult> SubmitReview([FromBody] CreateReviewDTO createReviewDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var reviwerId = GetUserId();

        try
        {
            var review = await _reviewService.SubmitReviewAsync(reviwerId, createReviewDto);
            
            return StatusCode(201, new { message = "Review submitted successfully.", reviewId = review.ReviewId });        
        } 
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message); 
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error submitting review for booking {BookingId}", createReviewDto.BookingId);
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }
    
    [HttpPost("report")]
    public async Task<IActionResult> ReportReview([FromBody] ReportReviewDTO dto)
    {
        var reporterId = GetUserId();
    
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
    
        try
        {
            await _reportService.ReportReviewAsync(reporterId, dto.ReviewId, dto.Reason);
            return Ok(new { message = "Review reported successfully for moderator review." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reporting review {ReviewId}", dto.ReviewId);
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }
}