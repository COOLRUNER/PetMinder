using Microsoft.EntityFrameworkCore;
using PetMinder.Data;
using PetMinder.Models;
using PetMinder.Shared.DTO;
using WebApplication1.Services.Interfaces;

namespace WebApplication1.Services;

public class NotificationService : INotificationService
{
    private readonly ApplicationDbContext _context;

    public NotificationService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task CreateNotificationAsync(long userId, NotificationType notificationType,
        string content, long? bookingId = null, long? messageId = null)
    {
        var isEnabled = await _context.NotificationPreferences
            .Where(p => p.UserId == userId && p.NotificationType == notificationType)
            .Select(p => p.IsEnabled)
            .Cast<bool?>()
            .FirstOrDefaultAsync() ?? true;

        if (!isEnabled) return;

        var notification = new Notification
        {
            UserId = userId,
            NotificationType = notificationType,
            Content = content,
            BookingId = bookingId,
            MessageId = messageId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();
    }

    public async Task<List<NotificationDTO>> GetUserNotificationsAsync(long userId)
    {
        return await _context.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .Select(n => new NotificationDTO
            {
                NotificationId = n.NotificationId,
                NotificationType = n.NotificationType,
                Content = n.Content,
                CreatedAt = n.CreatedAt,
                BookingId = n.BookingId,
                MessageId = n.MessageId
            })
            .ToListAsync();
    }

    public async Task<int> GetNotificationCountAsync(long userId)
    {
        return await _context.Notifications
            .Where(n => n.UserId == userId)
            .CountAsync();
    }

    public async Task DeleteNotificationAsync(long notificationId, long userId)
    {
        var notification = await _context.Notifications
            .FirstOrDefaultAsync(n => n.NotificationId == notificationId && n.UserId == userId);

        if (notification != null)
        {
            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteAllNotificationsAsync(long userId)
    {
        var notifications = await _context.Notifications
            .Where(n => n.UserId == userId)
            .ToListAsync();

        _context.Notifications.RemoveRange(notifications);
        await _context.SaveChangesAsync();
    }

    public async Task<List<NotificationPreferenceDTO>> GetPreferencesAsync(long userId)
    {
        var existingPreferences = await _context.NotificationPreferences
            .Where(p => p.UserId == userId)
            .ToListAsync();

        var allTypes = Enum.GetValues<NotificationType>();
        var result = new List<NotificationPreferenceDTO>();

        foreach (var type in allTypes)
        {
            var pref = existingPreferences.FirstOrDefault(p => p.NotificationType == type);
            result.Add(new NotificationPreferenceDTO
            {
                NotificationType = type,
                IsEnabled = pref?.IsEnabled ?? true
            });
        }

        return result;
    }

    public async Task UpdatePreferenceAsync(long userId, NotificationType type, bool isEnabled)
    {
        var pref = await _context.NotificationPreferences
            .FirstOrDefaultAsync(p => p.UserId == userId && p.NotificationType == type);

        if (pref == null)
        {
            pref = new NotificationPreference
            {
                UserId = userId,
                NotificationType = type,
                IsEnabled = isEnabled
            };
            _context.NotificationPreferences.Add(pref);
        }
        else
        {
            pref.IsEnabled = isEnabled;
        }

        await _context.SaveChangesAsync();
    }
}