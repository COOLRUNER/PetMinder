using System.Net.Http.Json;
using PetMinder.Shared.DTO;

namespace PetMinder.Client.Services;

public class NotificationService
{
    private readonly HttpClient _httpClient;

    public NotificationService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("AuthApi");
    }

    public async Task<List<NotificationDTO>> GetNotificationsAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<NotificationDTO>>("api/notifications")
               ?? new List<NotificationDTO>();
    }

    public async Task<int> GetNotificationCountAsync()
    {
        var result = await _httpClient.GetFromJsonAsync<NotificationCountResponse>("api/notifications/count");
        return result?.Count ?? 0;
    }

    public async Task DeleteNotificationAsync(long notificationId)
    {
        await _httpClient.DeleteAsync($"api/notifications/{notificationId}");
    }

    public async Task DeleteAllNotificationsAsync()
    {
        await _httpClient.DeleteAsync("api/notifications");
    }

    public async Task<List<NotificationPreferenceDTO>> GetPreferencesAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<NotificationPreferenceDTO>>("api/notifications/preferences")
               ?? new List<NotificationPreferenceDTO>();
    }

    public async Task UpdatePreferenceAsync(NotificationPreferenceDTO preference)
    {
        await _httpClient.PutAsJsonAsync("api/notifications/preferences", preference);
    }

    private class NotificationCountResponse
    {
        public int Count { get; set; }
    }
}
