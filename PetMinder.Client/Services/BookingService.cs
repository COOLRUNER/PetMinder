using System.Net.Http.Json;
using PetMinder.Shared.DTO;
using System.Net;
using System.Text.Json; 

namespace PetMinder.Client.Services
{
    public class BookingService
    {
        private readonly HttpClient _httpClient;

        public BookingService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("AuthApi");
        }

        public async Task<ServiceResult<bool>> CompleteBookingAsync(long bookingId)
        {
            try
            {
                var response = await _httpClient.PostAsync($"api/bookings/{bookingId}/complete", null);
                if (response.IsSuccessStatusCode)
                {
                    return new ServiceResult<bool> { IsSuccess = true, Data = true };
                }

                var error = await response.Content.ReadAsStringAsync();
                return new ServiceResult<bool> { IsSuccess = false, ErrorMessage = await ParseErrorMessage(response.StatusCode, error) };
            }
            catch (Exception ex)
            {
                return new ServiceResult<bool> { IsSuccess = false, ErrorMessage = ex.Message };
            }
        }

        public class ServiceResult<T>
        {
            public T? Data { get; set; }
            public bool IsSuccess { get; set; }
            public string? ErrorMessage { get; set; }
            public HttpStatusCode StatusCode { get; set; }
        }

        public async Task<ServiceResult<BookingRequestDTO>> CreateBookingRequestAsync(CreateBookingRequestDTO dto)
        {
            var result = new ServiceResult<BookingRequestDTO>();
            try
            {
                dto.StartTime = dto.StartTime.ToUniversalTime();
                dto.EndTime = dto.EndTime.ToUniversalTime();

                var response = await _httpClient.PostAsJsonAsync("api/bookings", dto);
                result.StatusCode = response.StatusCode;

                if (response.IsSuccessStatusCode)
                {
                    result.IsSuccess = true;
                    result.Data = await response.Content.ReadFromJsonAsync<BookingRequestDTO>();
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
                result.ErrorMessage = $"Network error or unexpected exception: {ex.Message}";
                result.StatusCode = HttpStatusCode.InternalServerError; 
            }
            return result;
        }

        public async Task<ServiceResult<List<BookingRequestDTO>>> GetOwnerBookingRequestsAsync()
        {
            var result = new ServiceResult<List<BookingRequestDTO>>();
            try
            {
                var response = await _httpClient.GetAsync("api/bookings/owner/me");
                result.StatusCode = response.StatusCode;

                if (response.IsSuccessStatusCode)
                {
                    result.IsSuccess = true;
                    result.Data = await response.Content.ReadFromJsonAsync<List<BookingRequestDTO>>();
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
                result.ErrorMessage = $"Network error or unexpected exception: {ex.Message}";
                result.StatusCode = HttpStatusCode.InternalServerError;
            }
            return result;
        }

        public async Task<ServiceResult<List<BookingRequestDTO>>> GetSitterBookingRequestsAsync()
        {
            var result = new ServiceResult<List<BookingRequestDTO>>();
            try
            {
                var response = await _httpClient.GetAsync("api/bookings/sitter/me");
                result.StatusCode = response.StatusCode;

                if (response.IsSuccessStatusCode)
                {
                    result.IsSuccess = true;
                    result.Data = await response.Content.ReadFromJsonAsync<List<BookingRequestDTO>>();
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
                result.ErrorMessage = $"Network error or unexpected exception: {ex.Message}";
                result.StatusCode = HttpStatusCode.InternalServerError;
            }
            return result;
        }

        public async Task<ServiceResult<bool>> UpdateBookingRequestStatusAsync(long bookingId, UpdateBookingRequestStatusDTO dto)
        {
            var result = new ServiceResult<bool>();
            try
            {
                var response = await _httpClient.PatchAsJsonAsync($"api/bookings/{bookingId}/status", dto);
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
                    result.ErrorMessage = await ParseErrorMessage(response.StatusCode, errorContent);
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Data = false;
                result.ErrorMessage = $"Network error or unexpected exception: {ex.Message}";
                result.StatusCode = HttpStatusCode.InternalServerError;
            }
            return result;
        }

        public async Task<ServiceResult<bool>> ProposeBookingChangeAsync(long bookingId, UpdateBookingChangeDTO dto)
        {
            var result = new ServiceResult<bool>();
            try
            {
                dto.ProposedStartTime = dto.ProposedStartTime.ToUniversalTime();
                dto.ProposedEndTime = dto.ProposedEndTime.ToUniversalTime();

                var response = await _httpClient.PostAsJsonAsync($"api/bookings/{bookingId}/change", dto);
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
                    result.ErrorMessage = await ParseErrorMessage(response.StatusCode, errorContent);
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Data = false;
                result.ErrorMessage = $"Network error or unexpected exception: {ex.Message}";
                result.StatusCode = HttpStatusCode.InternalServerError;
            }
            return result;
        }

        public async Task<ServiceResult<bool>> CancelBookingAsync(CreateCancellationDTO dto)
        {
            var result = new ServiceResult<bool>();
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"api/bookings/{dto.BookingId}/cancel", dto);
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
                    result.ErrorMessage = await ParseErrorMessage(response.StatusCode, errorContent);
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Data = false;
                result.ErrorMessage = $"Network error or unexpected exception: {ex.Message}";
                result.StatusCode = HttpStatusCode.InternalServerError;
            }
            return result;
        }

        public async Task<ServiceResult<bool>> AcceptBookingChangeAsync(BookingChangeAcceptDTO dto)
        {
            var result = new ServiceResult<bool>();
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/bookings/accept-change", dto);
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
                    result.ErrorMessage = await ParseErrorMessage(response.StatusCode, errorContent);
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Data = false;
                result.ErrorMessage = $"Network error or unexpected exception: {ex.Message}";
                result.StatusCode = HttpStatusCode.InternalServerError;
            }
            return result;
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
                    return string.Join("\n", errors);
                }
            }
            catch (JsonException)
            {
            }
            return $"Server responded with status {statusCode} and content: {errorContent}";
        }
    }
}