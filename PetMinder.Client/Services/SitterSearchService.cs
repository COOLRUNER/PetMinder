using System.Net.Http.Json;
using System.Text.Json;
using PetMinder.Shared.DTO;

namespace PetMinder.Client.Services;

public class SitterSearchService
{
    private readonly HttpClient _httpClient;
    
    public SitterSearchService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("AuthApi");
    }

    public async Task<SearchSitterServiceResult<List<UserProfileDTO>>> SearchAvailableSittersAsync(SitterSearchRequestDTO searchRequest)
    {
        var result = new SearchSitterServiceResult<List<UserProfileDTO>>();
        try
        {
            var requestDto = new SitterSearchRequestDTO
            {
                DesiredStartTime = searchRequest.DesiredStartTime.ToUniversalTime(),
                DesiredEndTime = searchRequest.DesiredEndTime.ToUniversalTime(),
                SelectedPetIds = searchRequest.SelectedPetIds,
                SearchFromAddressId = searchRequest.SearchFromAddressId
            };

            var queryStringParts = new List<string>
            {
                $"DesiredStartTime={requestDto.DesiredStartTime:yyyy-MM-ddTHH:mm:ssZ}",
                $"DesiredEndTime={requestDto.DesiredEndTime:yyyy-MM-ddTHH:mm:ssZ}",
                $"SearchFromAddressId={requestDto.SearchFromAddressId}"
            };

            foreach (var petId in requestDto.SelectedPetIds)
            {
                queryStringParts.Add($"SelectedPetIds={petId}");
            }

            var queryString = string.Join("&", queryStringParts);

            var response = await _httpClient.GetAsync($"api/SitterAvailabilities/search?{queryString}");

            result.StatusCode = response.StatusCode;

            if (response.IsSuccessStatusCode)
            {
                result.IsSuccess = true;
                result.Data = await response.Content.ReadFromJsonAsync<List<UserProfileDTO>>();
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
                    else if (jsonDoc.RootElement.TryGetProperty("errors", out var errorsElement))
                    {
                        var errors = new List<string>();
                        foreach (var prop in errorsElement.EnumerateObject())
                        {
                            foreach (var error in prop.Value.EnumerateArray())
                            {
                                errors.Add(error.GetString() ?? "Validation error");
                            }
                        }
                        result.ErrorMessage = string.Join("\n", errors);
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
    
    public async Task<SearchSitterServiceResult<SearchSitterProfileDetailsDTO>> GetSitterDetailsAsync(long sitterId)
    {
        var result = new SearchSitterServiceResult<SearchSitterProfileDetailsDTO>();
        try
        {
            var response = await _httpClient.GetAsync($"api/users/{sitterId}/profile");

            result.StatusCode = response.StatusCode;

            if (response.IsSuccessStatusCode)
            {
                result.IsSuccess = true;
                result.Data = await response.Content.ReadFromJsonAsync<SearchSitterProfileDetailsDTO>();
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
                        result.ErrorMessage = $"Failed to load sitter details. Status: {response.StatusCode}. Content: {errorContent}";
                    }
                }
                catch (JsonException)
                {
                    result.ErrorMessage = $"Failed to load sitter details. Status: {response.StatusCode}. Non-JSON content: {errorContent}";
                }
            }
        }
        catch (Exception ex)
        {
            result.IsSuccess = false;
            result.ErrorMessage = $"Network error or unexpected exception fetching sitter details: {ex.Message}";
        }
        return result;
    }
}