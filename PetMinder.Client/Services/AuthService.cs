using System.Net.Http.Json;
using PetMinder.Shared.DTO;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using PetMinder.Models;
using PetMinder.Client.Services;

namespace PetMinder.Client.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;
        private readonly AuthenticationStateProvider _authStateProvider;

        public AuthService(HttpClient httpClient, ILocalStorageService localStorage, AuthenticationStateProvider authStateProvider)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
            _authStateProvider = authStateProvider;
        }

        public AuthenticationStateProvider AuthStateProvider => _authStateProvider;

        public async Task<bool> RegisterAsync(RegisterDTO dto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/register", dto);
            return response.IsSuccessStatusCode;
        }

        public async Task<AuthResultDTO?> LoginAsync(LoginDTO dto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", dto);
            if (!response.IsSuccessStatusCode) return null;
            var result = await response.Content.ReadFromJsonAsync<AuthResultDTO>();
            if (result != null && !string.IsNullOrEmpty(result.Token))
            {
                await _localStorage.SetItemAsync("authToken", result.Token);
                if (_authStateProvider is JwtAuthenticationStateProvider jwtProvider)
                {
                    jwtProvider.NotifyUserAuthentication(result.Token);
                }
            }
            return result;
        }

        public async Task LogoutAsync()
        {
            await _localStorage.RemoveItemAsync("authToken");
            if (_authStateProvider is JwtAuthenticationStateProvider jwtProvider)
            {
                jwtProvider.NotifyUserLogout();
            }
        }

        public async Task<string?> GetTokenAsync()
        {
            return await _localStorage.GetItemAsync<string>("authToken");
        }

        public async Task<PetMinder.Shared.DTO.UserProfileDTO?> GetUserProfileAsync()
        {
            return await _httpClient.GetFromJsonAsync<PetMinder.Shared.DTO.UserProfileDTO>("api/users/me");
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateUserProfileAsync(UpdateUserDTO dto)
        {
            var response = await _httpClient.PutAsJsonAsync("api/users/me", dto);

            if (response.IsSuccessStatusCode)
            {
                return (true, null);
            }

            try
            {
                var errorObj = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                if (errorObj != null && errorObj.TryGetValue("message", out var msg) && !string.IsNullOrWhiteSpace(msg))
                {
                    return (false, msg);
                }
            }
            catch
            {
            }

            return (false, $"Failed to update profile. Status code: {(int)response.StatusCode}");
        }

        public async Task<bool> SendVerificationEmailAsync()
        {
            var response = await _httpClient.PostAsync("api/auth/send-verification-email", null);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> VerifyEmailTokenAsync(Guid token)
        {
            var response = await _httpClient.PostAsync($"api/auth/verify-email?token={token}", null);
            return response.IsSuccessStatusCode;
        }

        public async Task<List<VerificationStep>?> GetVerificationStatusAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<List<VerificationStep>>("/api/auth/verification-status");
            return response;
        }

        public async Task<int?> GetUserPointsAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<int>("api/users/me/points");
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<TransactionDTO>?> GetMyTransactionsAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<TransactionDTO>>("api/users/me/transactions");
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<PetMinder.Shared.DTO.PointsLotDTO>?> GetMyPointsLotsAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<PetMinder.Shared.DTO.PointsLotDTO>>("api/users/me/points/lots");
            }
            catch
            {
                return null;
            }
        }
        
        public async Task<(bool Success, string Message)> ReportUserAsync(ReportUserDTO dto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/report-user", dto);
    
            if (response.IsSuccessStatusCode)
            {
                return (true, "Report submitted successfully.");
            }

            var errorContent = await response.Content.ReadAsStringAsync();
    
            try 
            {
                var errorObj = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(errorContent);
                if (errorObj != null && errorObj.TryGetValue("message", out var msg))
                {
                    return (false, msg);
                }
            }
            catch { }

            return (false, "An error occurred while submitting the report.");
        }
        public async Task<AuthResultDTO?> RefreshTokenAsync()
        {
            var response = await _httpClient.PostAsync("api/auth/refresh-token", null);
            if (!response.IsSuccessStatusCode) return null;
            
            var result = await response.Content.ReadFromJsonAsync<AuthResultDTO>();
            if (result != null && !string.IsNullOrEmpty(result.Token))
            {
                await _localStorage.SetItemAsync("authToken", result.Token);
                if (_authStateProvider is JwtAuthenticationStateProvider jwtProvider)
                {
                    jwtProvider.NotifyUserAuthentication(result.Token);
                }
            }
            return result;
        }
    }
}
