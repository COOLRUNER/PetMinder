using System.ComponentModel.DataAnnotations;

namespace PetMinder.Shared.DTO
{
    public class CreateReviewDTO
    {
        [Required(ErrorMessage = "Booking ID is required.")]
        public long BookingId { get; set; }

        [Required(ErrorMessage = "Comment is required.")]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "Comment must be between 10 and 500 characters.")]
        public string Comment { get; set; } = string.Empty;
        
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int? SitterRating { get; set; }

        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int? OwnerRating { get; set; }

        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int? PetRating { get; set; }
        
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int? HouseRating { get; set; }
        
        public long? PetId { get; set; }
    }

    public class ReviewDTO
    {
        public long ReviewId { get; set; }
        
        public long ReviewerId { get; set; }
        
        public string ReviewerName { get; set; } 
        
        public string Comment { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public long BookingId { get; set; }
        
        public int? HouseRating { get; set; }
        
        public int? SitterRating { get; set; }
        
        public int? OwnerRating { get; set; }
        
        public int? PetRating { get; set; }
    }
    
    public class UserAggregatedRatingDTO
    {
        public double AverageSitterRating { get; set; }
        public double AverageOwnerRating { get; set; }
        public int SitterReviewCount { get; set; }
        public int OwnerReviewCount { get; set; }
        public bool IsTrustedSitter { get; set; }
    }

    public class ReportReviewDTO
    {
        public long ReviewId { get; set; } 
        [Required] public string Reason { get; set; }
    }
}

