using System.ComponentModel.DataAnnotations;

namespace PetMinder.Shared.DTO;

public class ReportUserDTO
{
    [Required]
    public long ReportedUserId { get; set; }

    [Required]
    [StringLength(500, MinimumLength = 5, ErrorMessage = "Reason must be between 5 and 500 characters.")]
    public string Reason { get; set; }

    [Required]
    public string Source { get; set; }
}