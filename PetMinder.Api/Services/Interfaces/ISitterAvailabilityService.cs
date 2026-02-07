using PetMinder.Shared.DTO;

namespace WebApplication1.Services.Interfaces;

public interface ISitterAvailabilityService
{
    Task<SitterAvailabilityDTO> AddSitterAvailabilityAsync(long sitterId, AddSitterAvailabilityDTO dto);
    Task<List<SitterAvailabilityDTO>> GetSitterAvailabilitiesAsync(long sitterId);
    Task<SitterAvailabilityDTO> GetSitterAvailabilityByIdAsync(long availabilityId);
    Task<bool> UpdateSitterAvailabilityAsync(long sitterId, long availabilityId, UpdateSitterAvailabilityDTO dto);
    Task<bool> DeleteSitterAvailabilityAsync(long sitterId, long availabilityId);
    Task<List<UserProfileDTO>> FindAvailableSittersAsync(SitterSearchRequestDTO searchRequest, long ownerId);
}