using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetMinder.Models
{
    public class HouseListing
    {
        [Key]
        public long ListingId { get; set; }

        [Required]
        [ForeignKey(nameof(Owner))]
        public long OwnerId { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public CheckList CheckList { get; set; } = new CheckList();

        public virtual User Owner { get; set; }

        public virtual ICollection<HousePhoto> Photos { get; set; } = new List<HousePhoto>();

        public virtual ICollection<HouseIssueReport> IssueReports { get; set; } = new List<HouseIssueReport>();

    }
}
