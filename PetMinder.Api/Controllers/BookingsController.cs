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
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public BookingsController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    private long GetUserId() => long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

    [HttpPost]
    [Authorize(Roles = nameof(UserRole.Owner))]
    public async Task<IActionResult> CreateBookingRequest([FromBody] CreateBookingRequestDTO dto)
    {
        var ownerId = GetUserId();
        try
        {
            var booking = await _bookingService.CreateBookingRequestAsync(ownerId, dto);
            return CreatedAtAction(nameof(GetBookingRequestById), new { bookingId = booking.BookingId }, booking);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error creating booking request.", details = ex.Message });
        }
    }

    [HttpGet("{bookingId}")]
    public async Task<IActionResult> GetBookingRequestById(long bookingId)
    {
        var userId = GetUserId();
        var booking = await _bookingService.GetBookingRequestByIdAsync(bookingId);

        if (booking == null)
        {
            return NotFound("Booking request not found.");
        }

        if (booking.OwnerId != userId && booking.SitterId != userId)
        {
            return Forbid();
        }

        return Ok(booking);
    }

    [HttpGet("owner/me")]
    [Authorize(Roles = nameof(UserRole.Owner))]
    public async Task<IActionResult> GetMyOwnerBookingRequests()
    {
        var ownerId = GetUserId();
        var bookings = await _bookingService.GetOwnerBookingRequestsAsync(ownerId);
        return Ok(bookings);
    }

    [HttpGet("sitter/me")]
    [Authorize(Roles = nameof(UserRole.Sitter))]
    public async Task<IActionResult> GetMySitterBookingRequests()
    {
        var sitterId = GetUserId();
        var bookings = await _bookingService.GetSitterBookingRequestsAsync(sitterId);
        return Ok(bookings);
    }

    [HttpPatch("{bookingId}/status")]
    [Authorize(Roles = nameof(UserRole.Sitter) + "," + nameof(UserRole.Admin))]
    public async Task<IActionResult> UpdateBookingStatus(long bookingId, [FromBody] UpdateBookingRequestStatusDTO updateDto)
    {
        var sitterId = GetUserId();
        if (sitterId == 0)
        {
            return Unauthorized("User ID not found in token or invalid token.");
        }

        if (updateDto.NewStatus != BookingStatus.Accepted && updateDto.NewStatus != BookingStatus.Rejected)
        {
            return BadRequest("Invalid status update. This endpoint only supports 'Accepted' or 'Rejected' statuses.");
        }

        try
        {
            var updatedBooking = await _bookingService.UpdateBookingStatusAsync(bookingId, sitterId, updateDto);

            if (updatedBooking == null)
            {
                return NotFound($"Booking request with ID {bookingId} not found.");
            }

            return Ok(new { message = $"Booking request {updatedBooking.BookingId} status updated to {updatedBooking.Status}." });
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new { message = ex.Message });
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
            return StatusCode(500, new { message = "An error occurred while updating the booking status.", details = ex.Message });
        }
    }

    [HttpPost("{bookingId}/change")]
    public async Task<IActionResult> ProposeBookingChange(long bookingId,
        [FromBody] UpdateBookingChangeDTO bookingChangeDto)
    {
        var senderId = GetUserId();

        bookingChangeDto.BookingId = bookingId;

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var change = await _bookingService.ProposeChangeAsync(senderId, bookingChangeDto);
            return Ok(new { messaage = "Change proposal sent successfully.", changeId = change.ChangeId });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = "Booking not found." });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Failed to create proposal.", details = ex.Message });
        }
    }

    [HttpPost("accept-change")]
    public async Task<IActionResult> AcceptBookingChange([FromBody] BookingChangeAcceptDTO changeAcceptDto)
    {
        var receiverId = GetUserId();

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var booking = await _bookingService.AcceptChangeAsync(receiverId, changeAcceptDto);
            return Ok(new { message = "Change proposal has been successfully accepted", bookingId = booking.BookingId });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Failed to  accept booking change.", details = ex.Message });
        }
    }

    [HttpPost("{bookingId}/cancel")]
    public async Task<IActionResult> CancelBooking(long bookingId, [FromBody] CreateCancellationDTO cancellationDto)
    {
        var userId = GetUserId();
        cancellationDto.BookingId = bookingId;

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var cancellation = await _bookingService.CancelBookingAsync(userId, cancellationDto);

            return Ok(new
            {
                message = "Booking cancellation was successful and refund processed.",
                cancellationId = cancellation.CancellationId
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Booking cancellation failed.", details = ex.Message });
        }
    }
    [HttpPost("{bookingId}/complete")]
    public async Task<IActionResult> CompleteBooking(long bookingId)
    {
        var userId = GetUserId();
        try
        {
            var booking = await _bookingService.CompleteBookingAsync(userId, bookingId);
            return Ok(new { message = "Booking marked as completed.", booking });
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
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error completing booking.", details = ex.Message });
        }
    }
}