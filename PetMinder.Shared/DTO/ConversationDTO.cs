using System;
using System.Collections.Generic;

namespace PetMinder.Shared.DTO
{
    public class ConversationDTO
    {
        public long ConversationId { get; set; }
        public long? BookingId { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<ConversationParticipantDTO> Participants { get; set; } = new();
        public MessageDTO? LastMessage { get; set; }
    }

    public class ConversationParticipantDTO
    {
        public long ParticipantId { get; set; }
        public long UserId { get; set; }
        public string? UserName { get; set; }
        public DateTime JoinedAt { get; set; }
        public string? ProfilePhotoUrl { get; set; }
    }

    public class MessageDTO
    {
        public long MessageId { get; set; }
        public long SenderId { get; set; }
        public string? SenderName { get; set; }
        public long ConversationId { get; set; }
        public string Content { get; set; }
        public DateTime SentAt { get; set; }
    }
}
