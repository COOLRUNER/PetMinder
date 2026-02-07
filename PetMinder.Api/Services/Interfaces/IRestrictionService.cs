using PetMinder.Shared.DTO;

namespace WebApplication1.Services.Interfaces;

public interface IRestrictionService
{
    Task<List<RestrictionTypeDTO>> GetAllRestrictionTypesAsync();
    Task<SitterRestrictionDTO> AddSitterRestrictionAsync(long sitterId, AddSitterRestrictionDTO dto);
    Task<bool> RemoveSitterRestrictionAsync(long sitterId, long restrictionTypeId);
    Task<List<SitterRestrictionDTO>> GetSitterRestrictionsAsync(long sitterId);
}