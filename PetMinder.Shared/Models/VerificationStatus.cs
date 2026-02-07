using System.ComponentModel.DataAnnotations;

namespace PetMinder.Models
{
    public class VerificationStatus
    {
        [Key]
        public long VerificationStatusId { get; set; }
        [Required]
        public VerificationStatusName Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<UserVerification> UserVerifications { get; set; } = new List<UserVerification>();
    }
}
