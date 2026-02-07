using System.Net.Http.Json;
using PetMinder.Shared.DTO;
using System.Text.Json;

namespace PetMinder.Client.Services
{
    public class SitterAvailabilityService
    {
        private readonly HttpClient _httpClient;

        public SitterAvailabilityService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("AuthApi");
        }

        public async Task<List<SitterAvailabilityDTO>?> GetMySitterAvailabilitiesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/SitterAvailabilities/me");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<List<SitterAvailabilityDTO>>();
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error getting my sitter availabilities: {ex.Message}");
                return null;
            }
        }

        public async Task<List<SitterAvailabilityDTO>?> GetSitterAvailabilitiesAsync(long sitterId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/SitterAvailabilities/{sitterId}");
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new List<SitterAvailabilityDTO>();
                }
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<List<SitterAvailabilityDTO>>();
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error getting sitter {sitterId} availabilities: {ex.Message}");
                return null;
            }
        }

        public async Task< SearchSitterServiceResult<SitterAvailabilityDTO>> AddSitterAvailabilityAsync(AddSitterAvailabilityDTO dto)
        {
            var result = new  SearchSitterServiceResult<SitterAvailabilityDTO>();
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/SitterAvailabilities", dto);
                result.StatusCode = response.StatusCode;

                if (response.IsSuccessStatusCode)
                {
                    result.IsSuccess = true;
                    result.Data = await response.Content.ReadFromJsonAsync<SitterAvailabilityDTO>();
                }
                else
                {
                    result.IsSuccess = false;
                    var errorContent = await response.Content.ReadAsStringAsync();
                    try
                    {
                        var jsonDoc = JsonDocument.Parse(errorContent);
                        if (jsonDoc.RootElement.TryGetProperty("message", out var messageElement))
                        {
                            result.ErrorMessage = messageElement.GetString();
                        }
                        else
                        {
                            result.ErrorMessage = $"Server responded with status {response.StatusCode} and content: {errorContent}";
                        }
                    }
                    catch (JsonException)
                    {
                        result.ErrorMessage = $"Server responded with status {response.StatusCode} and non-JSON content: {errorContent}";
                    }
                }
            }
            catch (Exception ex) 
            {
                result.IsSuccess = false;
                result.ErrorMessage = $"Network error or unexpected exception: {ex.Message}";
            }
            return result;
        }

        public async Task< SearchSitterServiceResult<bool>> UpdateSitterAvailabilityAsync(long availabilityId, UpdateSitterAvailabilityDTO dto)
        {
            var result = new  SearchSitterServiceResult<bool>();
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/SitterAvailabilities/{availabilityId}", dto);
                result.StatusCode = response.StatusCode;

                if (response.IsSuccessStatusCode)
                {
                    result.IsSuccess = true;
                    result.Data = true; 
                }
                else
                {
                    result.IsSuccess = false;
                    result.Data = false;
                    var errorContent = await response.Content.ReadAsStringAsync();
                    try
                    {
                        var jsonDoc = JsonDocument.Parse(errorContent);
                        if (jsonDoc.RootElement.TryGetProperty("message", out var messageElement))
                        {
                            result.ErrorMessage = messageElement.GetString();
                        }
                        else
                        {
                            result.ErrorMessage = $"Server responded with status {response.StatusCode} and content: {errorContent}";
                        }
                    }
                    catch (JsonException)
                    {
                        result.ErrorMessage = $"Server responded with status {response.StatusCode} and non-JSON content: {errorContent}";
                    }
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.ErrorMessage = $"Network error or unexpected exception: {ex.Message}";
                result.Data = false;
            }
            return result;
        }

        public async Task<bool> DeleteSitterAvailabilityAsync(long availabilityId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/SitterAvailabilities/{availabilityId}");
                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error deleting sitter availability {availabilityId}: {ex.Message}");
                return false;
            }
        }
    }
}