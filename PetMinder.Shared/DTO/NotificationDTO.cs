using PetMinder.Models;

namespace PetMinder.Shared.DTO;

public class NotificationDTO
{
    public long NotificationId { get; set; }
    public NotificationType NotificationType { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public long? BookingId { get; set; }
    public long? MessageId { get; set; }
}
