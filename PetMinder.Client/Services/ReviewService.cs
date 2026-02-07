using System.Net.Http.Json;
using PetMinder.Shared.DTO;

namespace PetMinder.Client.Services;

public class ReviewService
{
    private readonly HttpClient _httpClient;

    public ReviewService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("AuthApi");
    }

    public async Task<bool> SubmitReviewAsync(CreateReviewDTO dto)
    {
        var response = await _httpClient.PostAsJsonAsync("api/reviews", dto);
        return response.IsSuccessStatusCode;
    }
    
    public async Task<bool> ReportReviewAsync(ReportReviewDTO dto)
    {
        var response = await _httpClient.PostAsJsonAsync("api/reviews/report", dto);
        return response.IsSuccessStatusCode;
    }
}
