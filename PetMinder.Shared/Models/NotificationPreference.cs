using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetMinder.Models
{
    public class NotificationPreference
    {
        [Key]
        public long PreferenceId { get; set; }

        [Required]
        [ForeignKey(nameof(User))]
        public long UserId { get; set; }

        [Required]
        public NotificationType NotificationType { get; set; }

        [Required]
        public bool IsEnabled { get; set; } = true;

        public virtual User? User { get; set; }
    }
}
