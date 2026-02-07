using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetMinder.Models
{
    public class HouseIssueReport
    {
        [Key]
        public long IssueId { get; set; }

        [Required]
        [ForeignKey(nameof(Reporter))]
        public long ReporterId { get; set; }

        [Required]
        [ForeignKey(nameof(BookingRequest))]
        public long BookingId { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public DateTime ReportedAt { get; set; } = DateTime.UtcNow;

        public virtual User Reporter { get; set; }

        public virtual BookingRequest BookingRequest { get; set; }
    }
}
