using System.ComponentModel.DataAnnotations;

namespace PetMinder.Models
{
    public class PointPolicy
    {
        [Key]
        public long PolicyId { get; set; }

        [Required, StringLength(20)]
        public PointPolicyServiceType ServiceType { get; set; }

        [Required]
        public int PointsPerHour { get; set; }

        [Required]
        public int MinSpendable { get; set; }

        [Required]
        public int ExpiryDays { get; set; }

        public virtual ICollection<PointsTransaction> Transactions { get; set; } = new List<PointsTransaction>();
    }
}
