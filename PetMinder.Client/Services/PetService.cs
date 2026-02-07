using System.Net.Http.Json;
using PetMinder.Shared.DTO;

namespace PetMinder.Client.Services
{
    public class PetService
    {
        private readonly HttpClient _httpClient;
        public PetService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("AuthApi");
        }

        public async Task<List<PetDTO>?> GetPetsAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<PetDTO>>("api/pets");
        }

        public async Task<PetDTO?> AddPetAsync(CreatePetDTO dto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/pets", dto);
            return await response.Content.ReadFromJsonAsync<PetDTO>();
        }

        public async Task<PetDTO?> UpdatePetAsync(UpdatePetDTO dto)
        {
            var response = await _httpClient.PutAsJsonAsync("api/pets", dto);
            return await response.Content.ReadFromJsonAsync<PetDTO>();
        }

        public async Task<bool> DeletePetAsync(long petId)
        {
            var response = await _httpClient.DeleteAsync($"api/pets/{petId}");
            return response.IsSuccessStatusCode;
        }
    }
}
