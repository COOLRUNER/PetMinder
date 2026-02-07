using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetMinder.Models
{
    public class UserVerification
    {
        [Key]
        public long UserVerificationId { get; set; }

        [Required]
        [ForeignKey(nameof(User))]
        public long UserId { get; set; }

        [Required]
        [ForeignKey(nameof(VerificationStatus))]
        public long VerificationStatusId { get; set; }

        [Required]
        public DateTime VerifiedAt { get; set; } = DateTime.UtcNow;

        public string DocumentUrl { get; set; }

        public virtual User User { get; set; }

        public virtual VerificationStatus VerificationStatus { get; set; }
    }
}
