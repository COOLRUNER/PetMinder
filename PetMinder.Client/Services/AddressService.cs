using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using PetMinder.Shared.DTO;

namespace PetMinder.Client.Services;

public class AddressService
{
    private readonly HttpClient _httpClient;
    
    public AddressService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("AuthApi");
    }
    
    public async Task<List<AddressDTO>> GetUserAddressesAsync()
    {
        try
        {
            var addresses = await _httpClient.GetFromJsonAsync<List<AddressDTO>>("api/Address/me");
            return addresses ?? new List<AddressDTO>();
        }
        catch
        {
            return new List<AddressDTO>();
        }
    }
    
    public async Task<SearchSitterServiceResult<AddressDTO>> AddAddressAsync(CreateAddressDTO dto)
    {
        var result = new SearchSitterServiceResult<AddressDTO>();
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/Address", dto);
            result.StatusCode = response.StatusCode;

            if (response.IsSuccessStatusCode)
            {
                result.IsSuccess = true;
                result.Data = await response.Content.ReadFromJsonAsync<AddressDTO>();
            }
            else
            {
                result.IsSuccess = false;
                var errorContent = await response.Content.ReadAsStringAsync();
                result.ErrorMessage = await ParseErrorMessage(response.StatusCode, errorContent);
            }
        }
        catch (Exception ex)
        {
            result.IsSuccess = false;
            result.ErrorMessage = $"Network error: {ex.Message}";
            result.StatusCode = HttpStatusCode.InternalServerError;
        }
        return result;
    }
    
    public async Task<bool> DeleteAddressAsync(long userAddressId)
    {
        var response = await _httpClient.DeleteAsync($"api/Address/{userAddressId}");
        return response.IsSuccessStatusCode;
    }
    
    private async Task<string> ParseErrorMessage(HttpStatusCode statusCode, string errorContent)
    {
        try
        {
            var jsonDoc = JsonDocument.Parse(errorContent);
            if (jsonDoc.RootElement.TryGetProperty("message", out var messageElement))
            {
                return messageElement.GetString() ?? $"Server error (Status: {statusCode})";
            }
        }
        catch (JsonException) { }
        return $"Server responded with status {statusCode}.";
    }
    
    
}