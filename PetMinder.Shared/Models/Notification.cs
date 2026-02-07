using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetMinder.Models
{
    public class Notification
    {
        [Key]
        public long NotificationId { get; set; }

        [ForeignKey(nameof(Message))]
        public long? MessageId { get; set; }

        [ForeignKey(nameof(BookingRequest))]
        public long? BookingId { get; set; }

        [Required]
        [ForeignKey(nameof(User))]
        public long UserId { get; set; }

        [Required]
        public NotificationType NotificationType { get; set; }

        [Required, StringLength(255)]
        public string Content { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual Message Message { get; set; }

        public virtual BookingRequest BookingRequest { get; set; }

        public virtual User User { get; set; }
    }
}
