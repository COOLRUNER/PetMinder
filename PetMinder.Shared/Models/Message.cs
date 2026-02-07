using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetMinder.Models
{
    public class Message
    {
        [Key]
        public long MessageId { get; set; }

        [Required]
        [ForeignKey(nameof(Sender))]
        public long SenderId { get; set; }

        [Required]
        [ForeignKey(nameof(Conversation))]
        public long ConversationId { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        public virtual User Sender { get; set; }

        public virtual Conversation Conversation { get; set; }

        public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    }
}
