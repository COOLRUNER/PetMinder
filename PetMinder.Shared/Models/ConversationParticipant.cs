using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetMinder.Models
{
    public class ConversationParticipant
    {
        [Key]
        public long ParticipantId { get; set; }

        [Required]
        [ForeignKey(nameof(Conversation))]
        public long ConversationId { get; set; }

        [Required]
        [ForeignKey(nameof(User))]
        public long UserId { get; set; }

        [Required]
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

        public virtual Conversation Conversation { get; set; }

        public virtual User User { get; set; }
    }
}
