using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetMinder.Api.Services;
using System.Security.Claims;

namespace PetMinder.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FileUploadController : ControllerBase
    {
        private readonly IFileUploadService _fileUploadService;

        public FileUploadController(IFileUploadService fileUploadService)
        {
            _fileUploadService = fileUploadService;
        }

        [HttpPost("profile-photo")]
        public async Task<IActionResult> UploadProfilePhoto(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file provided.");

            if (!_fileUploadService.IsValidImageFile(file))
                return BadRequest("Invalid file type or size. Only JPG, JPEG, PNG, GIF, and WEBP files under 5MB are allowed.");

            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                    return Unauthorized();

                var photoUrl = await _fileUploadService.UploadFileAsync(file, "profile-photos");
                
                return Ok(new { PhotoUrl = photoUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error uploading file: {ex.Message}");
            }
        }

        [HttpPost("pet-photo")]
        public async Task<IActionResult> UploadPetPhoto(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file provided.");

            if (!_fileUploadService.IsValidImageFile(file))
                return BadRequest("Invalid file type or size. Only JPG, JPEG, PNG, GIF, and WEBP files under 5MB are allowed.");

            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                    return Unauthorized();

                var photoUrl = await _fileUploadService.UploadFileAsync(file, "pet-photos");
                
                return Ok(new { PhotoUrl = photoUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error uploading file: {ex.Message}");
            }
        }

        [HttpDelete("delete-photo")]
        public async Task<IActionResult> DeletePhoto([FromQuery] string photoUrl)
        {
            if (string.IsNullOrEmpty(photoUrl))
                return BadRequest("Photo URL is required.");

            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                    return Unauthorized();

                var deleted = await _fileUploadService.DeleteFileAsync(photoUrl);
                
                if (deleted)
                    return Ok(new { Message = "Photo deleted successfully." });
                else
                    return NotFound("Photo not found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error deleting file: {ex.Message}");
            }
        }
    }
}