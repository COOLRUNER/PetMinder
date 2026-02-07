using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetMinder.Models
{
    public class Conversation
    {

        [Key]
        public long ConversationId { get; set; }

        [ForeignKey(nameof(BookingRequest))]
        public long? BookingId { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual BookingRequest BookingRequest { get; set; }

        public virtual ICollection<ConversationParticipant> Participants { get; set; } = new List<ConversationParticipant>();

        public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    }
}
