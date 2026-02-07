using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetMinder.Models
{
    public class DeviceFingerprint
    {
        [Key]
        public long FingerprintId { get; set; }
        [Required]
        [ForeignKey(nameof(User))]
        public long UserId { get; set; }
        [Required, StringLength(255)]
        public string Fingerprint { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual User User { get; set; }
    }
}
