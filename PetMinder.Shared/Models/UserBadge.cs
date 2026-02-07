using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetMinder.Models
{
    public class UserBadge
    {
        [Key]
        public long UserBadgeId { get; set; }

        [Required]
        [ForeignKey(nameof(User))]
        public long UserId { get; set; }

        [Required]
        [ForeignKey(nameof(Badge))]
        public long BadgeId { get; set; }

        [Required]
        public DateTime AwardedAt { get; set; } = DateTime.UtcNow;

        public virtual User User { get; set; }

        public virtual Badge Badge { get; set; }
    }
}