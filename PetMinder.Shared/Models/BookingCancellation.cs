using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetMinder.Models
{
    public class BookingCancellation
    {
        [Key]
        public long CancellationId { get; set; }

        [Required]
        [ForeignKey(nameof(BookingRequest))]
        public long BookingId { get; set; }

        [Required]
        [ForeignKey(nameof(CancellationPolicy))]
        public long PolicyId { get; set; }

        [Required]
        public CancelledBy CancelledBy { get; set; }

        [Required]
        public DateTime CancelledAt { get; set; } = DateTime.UtcNow;

        public virtual BookingRequest BookingRequest { get; set; }

        public virtual CancellationPolicy CancellationPolicy { get; set; }
    }
}
