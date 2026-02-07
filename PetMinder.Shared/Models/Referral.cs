using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetMinder.Models
{
    public class Referral
    {
        [Key]
        public long ReferralId { get; set; }

        [Required]
        [ForeignKey(nameof(Referrer))]
        public long ReferrerId { get; set; }

        [ForeignKey(nameof(ReferredUser))]
        public long? ReferredUserId { get; set; }

        [Required, StringLength(100)]
        public string Code { get; set; }

        [Required]
        public ReferralStatus Status { get; set; } = ReferralStatus.Pending;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? CompletedAt { get; set; }

        public virtual User Referrer { get; set; }

        public virtual User? ReferredUser { get; set; }
    }

    public enum ReferralStatus
    {
        Pending,
        Completed
    }
}
