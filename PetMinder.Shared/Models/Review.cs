using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetMinder.Models
{
    public class Review
    {
        [Key]
        public long ReviewId { get; set; }

        [Required]
        [ForeignKey(nameof(Reviewer))]
        public long ReviewerId { get; set; }

        [Required]
        [ForeignKey(nameof(Reviewee))]
        public long RevieweeId { get; set; }

        [ForeignKey(nameof(Pet))]
        public long? PetId;

        [Required]
        [ForeignKey(nameof(Booking))]
        public long BookingId { get; set; }

        [Range(1, 5)]
        public int? HouseRating { get; set; }

        [Range(1, 5)]
        public int? SitterRating { get; set; }

        [Range(1, 5)]
        public int? OwnerRating { get; set; }

        [Range(1, 5)]
        public int? PetRating { get; set; }

        public string Comment { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual User Reviewer { get; set; }
        public virtual User Reviewee { get; set; }
        public virtual Pet Pet { get; set; }
        public virtual BookingRequest Booking { get; set; }

        public virtual ICollection<ReviewReport> ReviewReports { get; set; } = new List<ReviewReport>();
    }
}
