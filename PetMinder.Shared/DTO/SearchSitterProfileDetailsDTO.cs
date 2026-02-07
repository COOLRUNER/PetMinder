namespace PetMinder.Shared.DTO;

public class SearchSitterProfileDetailsDTO
{
    public long UserId { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Phone { get; set; }
    public string? ProfilePhotoUrl { get; set; }
    public string[] Roles { get; set; } = System.Array.Empty<string>();
    public int MinPoints { get; set; } 

    public List<SitterAvailabilityDTO> Availabilities { get; set; } = new List<SitterAvailabilityDTO>();
    public List<SitterQualificationDTO> Qualifications { get; set; } = new List<SitterQualificationDTO>();
    public List<SitterRestrictionDTO> Restrictions { get; set; } = new List<SitterRestrictionDTO>();

    public double? DistanceInKm { get; set; }

    public double AverageSitterRating { get; set; }
    public int SitterReviewCount { get; set; }
    public double AverageOwnerRating { get; set; }
    public int OwnerReviewCount { get; set; }
    public List<ReviewDTO> RecentReviews { get; set; } = new List<ReviewDTO>();
    public List<BadgeDTO> Badges { get; set; } = new List<BadgeDTO>();
    public bool IsVerified { get; set; }
}