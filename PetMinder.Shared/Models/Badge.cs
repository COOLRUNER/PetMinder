using System.ComponentModel.DataAnnotations;

namespace PetMinder.Models
{
    public class Badge
    {
        [Key]
        public long BadgeId { get; set; }

        [Required, StringLength(50)]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public bool IsSpendable { get; set; } = false;

        public virtual ICollection<UserBadge> UserBadges { get; set; } = new List<UserBadge>();

    }
}
