using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetMinder.Models
{
    public class BookingRequest
    {
        [Key]
        public long BookingId { get; set; }

        [Required]
        [ForeignKey(nameof(Owner))]
        public long OwnerId { get; set; }

        [Required]
        [ForeignKey(nameof(Sitter))]
        public long SitterId { get; set; }

        [Required]
        [ForeignKey(nameof(Pet))]
        public long PetId { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        [Required]
        public int OfferedPoints { get; set; }

        [Required]
        public BookingStatus Status { get; set; } = BookingStatus.Pending;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public virtual User Owner { get; set; }

        public virtual User Sitter { get; set; }

        public virtual Pet Pet { get; set; }

        public virtual ICollection<BookingChange> BookingChanges { get; set; } = new List<BookingChange>();

        public virtual BookingCancellation Cancellation { get; set; }

        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

        public virtual ICollection<HouseIssueReport> HouseIssues { get; set; } = new List<HouseIssueReport>();

        public virtual Conversation Conversation { get; set; }

        public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    }
}
