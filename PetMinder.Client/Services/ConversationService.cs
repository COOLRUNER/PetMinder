using System.Net.Http;
using System.Net.Http.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using PetMinder.Shared.DTO;

namespace PetMinder.Client.Services
{
    public class ConversationService
    {
        private readonly HttpClient _httpClient;
        public ConversationService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("AuthApi");
        }

        public async Task<List<ConversationDTO>> GetUserConversationsAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<ConversationDTO>>("api/conversations")
                ?? new List<ConversationDTO>();
        }
    }
}
