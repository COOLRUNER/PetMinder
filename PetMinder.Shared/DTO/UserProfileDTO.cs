namespace PetMinder.Shared.DTO
{
    public class UserProfileDTO
    {
        public long UserId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string? ProfilePhotoUrl { get; set; }
        public string[] Roles { get; set; } = System.Array.Empty<string>();

        public double? DistanceInKm { get; set; }

        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }

        public double AverageOwnerRating { get; set; }
        public int OwnerReviewCount { get; set; }

        public int MinPoints { get; set; }
        
        public List<ReviewDTO> RecentReviews { get; set; } = new List<ReviewDTO>();
        public List<BadgeDTO> Badges { get; set; } = new List<BadgeDTO>();
        public string? ReferralCode { get; set; }
        public int ReferralsCount { get; set; }
        public bool IsVerified { get; set; }
    }
}
