using System.ComponentModel.DataAnnotations;

namespace PetMinder.Shared.DTO;

public class SitterSearchRequestDTO
{
    [Required(ErrorMessage = "Start time is required.")]
    public DateTime DesiredStartTime { get; set; }

    [Required(ErrorMessage = "End time is required.")]
    public DateTime DesiredEndTime { get; set; }
    
    [Required(ErrorMessage = "At least one pet must be selected.")]
    [MinLength(1, ErrorMessage = "At least one pet must be selected.")]
    public List<long> SelectedPetIds { get; set; } = new List<long>();
    
    [Required(ErrorMessage = "Please select an address to search from.")]
    [Range(1, long.MaxValue, ErrorMessage = "Please select a valid address to search from.")]
    public long SearchFromAddressId { get; set; }
}