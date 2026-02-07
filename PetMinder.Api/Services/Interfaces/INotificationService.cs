using PetMinder.Models;
using PetMinder.Shared.DTO;

namespace WebApplication1.Services.Interfaces;

public interface INotificationService
{
    Task CreateNotificationAsync(long userId, NotificationType notificationType, string content,
        long? bookingId = null, long? messageId = null);
    Task<List<NotificationDTO>> GetUserNotificationsAsync(long userId);
    Task<int> GetNotificationCountAsync(long userId);
    Task DeleteNotificationAsync(long notificationId, long userId);
    Task DeleteAllNotificationsAsync(long userId);
    Task<List<NotificationPreferenceDTO>> GetPreferencesAsync(long userId);
    Task UpdatePreferenceAsync(long userId, NotificationType type, bool isEnabled);
}
