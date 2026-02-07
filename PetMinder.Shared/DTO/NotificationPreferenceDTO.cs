using PetMinder.Models;

namespace PetMinder.Shared.DTO
{
    public class NotificationPreferenceDTO
    {
        public NotificationType NotificationType { get; set; }
        public bool IsEnabled { get; set; }
    }
}
