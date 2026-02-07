using System.ComponentModel.DataAnnotations;

namespace PetMinder.Models
{
    public class CancellationPolicy
    {
        [Key]
        public long PolicyId { get; set; }

        [Required]
        public CancellationPolicyName Name { get; set; }

        [Required]
        public int RefundPercentage { get; set; }

        public string Description { get; set; }

        public virtual ICollection<BookingCancellation> Cancellations { get; set; } = new List<BookingCancellation>();
    }
}