using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetMinder.Models
{
    public class BookingChange
    {
        [Key]
        public long ChangeId { get; set; }

        [Required]
        [ForeignKey(nameof(BookingRequest))]
        public long BookingId { get; set; }

        public DateTime? ProposedStart { get; set; }

        public DateTime? ProposedEnd { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public ChangeRequestedBy RequestedBy { get; set; }

        public virtual BookingRequest BookingRequest { get; set; }
    }
}
