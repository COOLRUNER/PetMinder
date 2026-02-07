using System.ComponentModel.DataAnnotations;

namespace PetMinder.Shared.DTO;

public class SitterAvailabilityDTO
{
    public long AvailabilityId { get; set; }
    public long SitterId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}

public class UpdateSitterAvailabilityDTO
{
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}

public class AddSitterAvailabilityDTO
{
    [Required(ErrorMessage = "Start time is required.")]
    public DateTime StartTime { get; set; }

    [Required(ErrorMessage = "End time is required.")]
    public DateTime EndTime { get; set; }
}