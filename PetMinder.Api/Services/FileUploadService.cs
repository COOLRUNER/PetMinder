using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.ComponentModel.DataAnnotations;

namespace PetMinder.Api.Services
{
    public interface IFileUploadService
    {
        Task<string> UploadFileAsync(IFormFile file, string folder);
        Task<bool> DeleteFileAsync(string fileUrl);
        bool IsValidImageFile(IFormFile file);
    }

    public class FileUploadService : IFileUploadService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<FileUploadService> _logger;
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        private const long MaxFileSize = 5 * 1024 * 1024; // 5MB
        private string _containerName;

        public FileUploadService(IConfiguration configuration, ILogger<FileUploadService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            
            var connectionString = _configuration["AZURE_STORAGE_CONNECTION_STRING"] 
                                   ?? _configuration.GetConnectionString("AzureStorage");
            
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Azure Storage connection string is missing.");
            }

            _blobServiceClient = new BlobServiceClient(connectionString);
            _containerName = _configuration["AZURE_STORAGE_CONTAINER_NAME"] ?? "uploads";
        }

        public async Task<string> UploadFileAsync(IFormFile file, string folder)
        {
            if (!IsValidImageFile(file))
            {
                throw new ValidationException("Invalid file type or size.");
            }

            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
                await containerClient.CreateIfNotExistsAsync();
                await containerClient.SetAccessPolicyAsync(PublicAccessType.Blob);

                var fileExtension = Path.GetExtension(file.FileName);
                var uniqueFileName = $"{folder}/{Guid.NewGuid()}{fileExtension}";
                var blobClient = containerClient.GetBlobClient(uniqueFileName);

                var httpHeaders = new BlobHttpHeaders 
                { 
                    ContentType = file.ContentType 
                };

                using (var stream = file.OpenReadStream())
                {
                    await blobClient.UploadAsync(stream, new BlobUploadOptions { HttpHeaders = httpHeaders });
                }

                return blobClient.Uri.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file {FileName} to Azure Blob Storage", file.FileName);
                throw;
            }
        }

        public async Task<bool> DeleteFileAsync(string fileUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(fileUrl))
                    return false;

                Uri uri = new Uri(fileUrl);
                string blobName = uri.Segments.Last();
                
                var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
                
                var path = uri.AbsolutePath; // /uploads/profile/guid.jpg
                var containerSegment = $"/{_containerName}/";
                var index = path.IndexOf(containerSegment, StringComparison.OrdinalIgnoreCase);
                
                if (index == -1) return false;
                
                var blobNameFromUrl = path.Substring(index + containerSegment.Length);
                var blobClient = containerClient.GetBlobClient(blobNameFromUrl);

                return await blobClient.DeleteIfExistsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting file {FileUrl}", fileUrl);
                return false;
            }
        }

        public bool IsValidImageFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return false;

            if (file.Length > MaxFileSize)
                return false;

            var fileExtension = Path.GetExtension(file.FileName)?.ToLowerInvariant();
            return !string.IsNullOrEmpty(fileExtension) && _allowedExtensions.Contains(fileExtension);
        }
    }
}