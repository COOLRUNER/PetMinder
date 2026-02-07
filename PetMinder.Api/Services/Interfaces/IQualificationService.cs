using PetMinder.Shared.DTO;

namespace WebApplication1.Services.Interfaces;

public interface IQualificationService
{
    Task<List<QualificationTypeDTO>> GetAllQualificationTypesAsync();
    Task<SitterQualificationDTO> AddSitterQualificationAsync(long sitterId, AddSitterQualificationDTO dto);
    Task<bool> RemoveSitterQualificationAsync(long sitterId, long qualificationTypeId);
    Task<List<SitterQualificationDTO>> GetSitterQualificationsAsync(long sitterId);
}