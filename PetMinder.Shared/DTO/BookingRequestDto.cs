using System.ComponentModel.DataAnnotations;
using PetMinder.Models;

namespace PetMinder.Shared.DTO;

public class BookingRequestDTO
{
    public long BookingId { get; set; }
    public long OwnerId { get; set; }
    public string OwnerFirstName { get; set; }
    public string OwnerLastName { get; set; }
    public long SitterId { get; set; }
    public string SitterFirstName { get; set; }
    public string SitterLastName { get; set; }
    public long PetId { get; set; }
    public string PetName { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int OfferedPoints { get; set; }
    public BookingStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Pending change proposal details (if any)
    public BookingChangeDTO? PendingChange { get; set; }

    public bool HasOwnerReviewed { get; set; }
    public bool HasSitterReviewed { get; set; }
}

public class BookingChangeDTO
{
    public long ChangeId { get; set; }
    public long BookingId { get; set; }
    public DateTime? ProposedStart { get; set; }
    public DateTime? ProposedEnd { get; set; }
    public DateTime CreatedAt { get; set; }
    public ChangeRequestedBy RequestedBy { get; set; }
}

public class CreateBookingRequestDTO
{
    [Required(ErrorMessage = "Sitter ID is required.")]
    public long SitterId { get; set; }

    [Required(ErrorMessage = "Pet ID is required.")]
    public long PetId { get; set; }

    [Required(ErrorMessage = "Start time is required.")]
    public DateTime StartTime { get; set; }

    [Required(ErrorMessage = "End time is required.")]
    public DateTime EndTime { get; set; }

    [Required(ErrorMessage = "Offered points are required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Offered points must be a positive number.")]
    public int OfferedPoints { get; set; }
}

public class UserSummaryDTO
{
    public long UserId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? ProfilePhotoUrl { get; set; }
}

public class UpdateBookingRequestStatusDTO
{
    [Required(ErrorMessage = "New status is required.")]
    [EnumDataType(typeof(BookingStatus), ErrorMessage = "Invalid booking status. Only Accepted or Rejected are allowed.")]
    public BookingStatus NewStatus { get; set; }

    public string? RejectionReason { get; set; }
}

public class UpdateBookingChangeDTO
{
    [Required(ErrorMessage = "Booking ID is required.")]
    public long BookingId { get; set; }

    [Required(ErrorMessage = "Proposed start time is required.")]
    public DateTime ProposedStartTime { get; set; }

    [Required(ErrorMessage = "Proposed end time is required.")]
    public DateTime ProposedEndTime { get; set; }
}

public class BookingChangeAcceptDTO
{
    [Required(ErrorMessage = "Change ID is required.")]
    public long ChangeId { get; set; }
}

public class CreateCancellationDTO
{
    [Required(ErrorMessage = "Booking ID is required.")]
    public long BookingId { get; set; }

    [Required(ErrorMessage = "Cancellation reason is required.")]
    [StringLength(500, MinimumLength = 10)]
    public string Reason { get; set; } = string.Empty;

    [Required(ErrorMessage = "Cancellation Policy ID is required.")]
    public long PolicyId { get; set; }
}