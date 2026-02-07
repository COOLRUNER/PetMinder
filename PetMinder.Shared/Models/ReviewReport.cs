using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Principal;

namespace PetMinder.Models
{
    public class ReviewReport
    {
        [Key]
        public long ReportId { get; set; }

        [Required]
        [ForeignKey(nameof(Review))]
        public long ReviewId { get; set; }

        [Required]
        [ForeignKey(nameof(Reporter))]
        public long ReporterId { get; set; }

        [Required, StringLength(255)]
        public string Reason { get; set; }

        [Required]
        public DateTime ReportedAt { get; set; } = DateTime.UtcNow;

        public virtual Review Review { get; set; }

        public virtual User Reporter { get; set; }
    }
}
