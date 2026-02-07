using System.Net.Http.Json;

namespace PetMinder.Client.Services
{
    public class FileUploadService
    {
        private readonly HttpClient _httpClient;

        public FileUploadService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string?> UploadProfilePhotoAsync(Stream fileStream, string fileName)
        {
            try
            {
                using var content = new MultipartFormDataContent();
                using var fileContent = new StreamContent(fileStream);
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(GetContentType(fileName));
                content.Add(fileContent, "file", fileName);

                var response = await _httpClient.PostAsync("api/fileupload/profile-photo", content);
                
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<UploadResult>();
                    return result?.PhotoUrl;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<string?> UploadPetPhotoAsync(Stream fileStream, string fileName)
        {
            try
            {
                using var content = new MultipartFormDataContent();
                using var fileContent = new StreamContent(fileStream);
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(GetContentType(fileName));
                content.Add(fileContent, "file", fileName);

                var response = await _httpClient.PostAsync("api/fileupload/pet-photo", content);
                
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<UploadResult>();
                    return result?.PhotoUrl;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> DeletePhotoAsync(string photoUrl)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/fileupload/delete-photo?photoUrl={Uri.EscapeDataString(photoUrl)}");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        private static string GetContentType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            return extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".webp" => "image/webp",
                _ => "application/octet-stream"
            };
        }

        private class UploadResult
        {
            public string PhotoUrl { get; set; } = string.Empty;
        }
    }
}