using System.Net.Http.Json;
using PetMinder.Shared.DTO;

namespace PetMinder.Client.Services
{
    public class SitterProfileService
    {
        private readonly HttpClient _httpClient;
        public SitterProfileService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("AuthApi");
        }
        public async Task<List<QualificationTypeDTO>?> GetQualificationTypesAsync()
            => await _httpClient.GetFromJsonAsync<List<QualificationTypeDTO>>("api/qualificationtypes");
        public async Task<List<SitterQualificationDTO>?> GetMyQualificationsAsync()
            => await _httpClient.GetFromJsonAsync<List<SitterQualificationDTO>>("api/sitterqualifications/me");

        public async Task<bool> AddQualificationAsync(AddSitterQualificationDTO dto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/sitterqualifications", dto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> RemoveQualificationAsync(long qualificationTypeId)
        {
            var response = await _httpClient.DeleteAsync($"api/sitterqualifications/{qualificationTypeId}");
            return response.IsSuccessStatusCode;
        }
        public async Task<List<RestrictionTypeDTO>?> GetRestrictionTypesAsync()
            => await _httpClient.GetFromJsonAsync<List<RestrictionTypeDTO>>("api/restrictiontypes");
        public async Task<List<SitterRestrictionDTO>?> GetMyRestrictionsAsync()
            => await _httpClient.GetFromJsonAsync<List<SitterRestrictionDTO>>("api/sitterrestrictions/me");

        public async Task<bool> AddRestrictionAsync(AddSitterRestrictionDTO dto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/sitterrestrictions", dto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> RemoveRestrictionAsync(long restrictionTypeId)
        {
            var response = await _httpClient.DeleteAsync($"api/sitterrestrictions/{restrictionTypeId}");
            return response.IsSuccessStatusCode;
        }
    }
}
