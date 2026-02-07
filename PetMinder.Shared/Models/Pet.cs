using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetMinder.Models
{
    public class Pet
    {
        [Key]
        public long PetId { get; set; }

        [Required]
        [ForeignKey(nameof(Owner))]
        public long UserId { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; }

        [Required, StringLength(100)]
        public string Breed { get; set; }

        [Required, Range(0, 100)]
        public int Age { get; set; }

        [Required]
        public PetType Type { get; set; }

        [Required]
        public PetBehaviorComplexity BehaviorComplexity { get; set; }

        public string? HealthNotes { get; set; }

        public string? BehaviorNotes { get; set; }

        [StringLength(512)]
        public string? PhotoUrl { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual User Owner { get; set; }

        public virtual ICollection<BookingRequest> BookingRequests { get; set; } = new List<BookingRequest>();

        public virtual ICollection<Review> PetReviews { get; set; } = new List<Review>();

    }
}
