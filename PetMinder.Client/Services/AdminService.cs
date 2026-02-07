using System.Net.Http.Json;
using PetMinder.Shared.DTO;

namespace PetMinder.Client.Services;

public class AdminService
{
    private readonly HttpClient _http;

    public AdminService(HttpClient http)
    {
        _http = http;
    }
    
    public async Task<List<AdminUserListDTO>> GetUsersAsync(string? search = null)
    {
        try
        {
            var url = "api/admin/users";
            if (!string.IsNullOrEmpty(search))
            {
                url += $"?search={Uri.EscapeDataString(search)}";
            }

            var result = await _http.GetFromJsonAsync<List<AdminUserListDTO>>(url);
            return result ?? new List<AdminUserListDTO>();
        }
        catch (Exception console)
        {
            Console.WriteLine($"Error fetching users: {console.Message}");
            return new List<AdminUserListDTO>();
        }
    }

    public async Task<bool> ToggleFlagUserAsync(long userId)
    {
        try
        {
            var response = await _http.PostAsync($"api/admin/users/{userId}/flag", null);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<AdminUserDetailsDTO?> GetUserDetailsAsync(long userId)
    {
        try
        {
            return await _http.GetFromJsonAsync<AdminUserDetailsDTO>($"api/admin/users/{userId}");
        }
        catch
        {
            return null;
        }
    }
    
    public async Task<List<AdminReportedReviewDTO>> GetReportedReviewsAsync()
    {
        try
        {
            return await _http.GetFromJsonAsync<List<AdminReportedReviewDTO>>("api/admin/reviews/reported") 
                   ?? new List<AdminReportedReviewDTO>();
        }
        catch
        {
            return new List<AdminReportedReviewDTO>();
        }
    }

    public async Task<bool> DeleteReviewAsync(long reviewId)
    {
        var response = await _http.DeleteAsync($"api/admin/reviews/{reviewId}");
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DismissReportsAsync(long reviewId)
    {
        var response = await _http.DeleteAsync($"api/admin/reviews/{reviewId}/reports");
        return response.IsSuccessStatusCode;
    }
    
    public async Task<List<ConversationDTO>> GetUserConversationsAsync(long userId)
    {
        try 
        {
            return await _http.GetFromJsonAsync<List<ConversationDTO>>($"api/admin/users/{userId}/conversations") 
                   ?? new List<ConversationDTO>();
        }
        catch 
        {
            return new List<ConversationDTO>();
        }
    }

    public async Task<List<MessageDTO>> GetConversationMessagesAsync(long conversationId)
    {
        try
        {
            return await _http.GetFromJsonAsync<List<MessageDTO>>($"api/admin/conversations/{conversationId}/messages")
                   ?? new List<MessageDTO>();
        }
        catch
        {
            return new List<MessageDTO>();
        }
    }
    
    public async Task<List<PointPolicyDTO>> GetPointPoliciesAsync()
    {
        try
        {
            return await _http.GetFromJsonAsync<List<PointPolicyDTO>>("api/admin/points/policies") 
                   ?? new List<PointPolicyDTO>();
        }
        catch
        {
            return new List<PointPolicyDTO>();
        }
    }
    
    public async Task<bool> UpdatePointPolicyAsync(PointPolicyDTO dto)
    {
        var response = await _http.PutAsJsonAsync("api/admin/points/policies", dto);
        return response.IsSuccessStatusCode;
    }
    
    public async Task<(bool Success, string Message)> ProcessRefundAsync(AdminTransactionAdjustmentDTO dto)
    {
        var response = await _http.PostAsJsonAsync("api/admin/points/refund", dto);
            
        if (response.IsSuccessStatusCode) 
        {
            return (true, "Refund processed successfully.");
        }

        var errorMsg = await response.Content.ReadAsStringAsync();
            
        if (errorMsg.Contains("message")) 
        {
            try {
                var errorObj = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(errorMsg);
                if(errorObj != null && errorObj.ContainsKey("message")) return (false, errorObj["message"]);
            } catch {}
        }
            
        return (false, string.IsNullOrWhiteSpace(errorMsg) ? "Refund failed." : errorMsg);
    }
    
    public async Task<List<AdminReportedUserDTO>> GetReportedUsersAsync()
    {
        try
        {
            return await _http.GetFromJsonAsync<List<AdminReportedUserDTO>>("api/admin/users/reported") 
                   ?? new List<AdminReportedUserDTO>();
        }
        catch
        {
            return new List<AdminReportedUserDTO>();
        }
    }

    public async Task<bool> BanUserAsync(long userId)
    {
        try 
        {
            var response = await _http.PostAsync($"api/admin/users/{userId}/ban", null);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> DismissUserReportsAsync(long userId)
    {
        try
        {
            var response = await _http.DeleteAsync($"api/admin/users/{userId}/reports");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
    
    public async Task<bool> DismissSingleUserReportAsync(long reportId)
    {
        try
        {
            var response = await _http.DeleteAsync($"api/admin/reports/{reportId}");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}