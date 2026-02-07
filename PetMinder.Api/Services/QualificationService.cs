using Microsoft.EntityFrameworkCore;
using PetMinder.Data;
using PetMinder.Models;
using PetMinder.Shared.DTO;
using WebApplication1.Services.Interfaces;

namespace WebApplication1.Services;

public class QualificationService : IQualificationService
{
    private readonly ApplicationDbContext _context;

    public QualificationService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<QualificationTypeDTO>> GetAllQualificationTypesAsync()
    {
        return await _context.QualificationTypes
            .Select(qt => new QualificationTypeDTO
            {
                QualificationTypeId = qt.QualificationTypeId,
                Code = qt.Code,
                Name = qt.Code, 
                Description = qt.Description
            })
            .ToListAsync();
    }

    public async Task<SitterQualificationDTO> AddSitterQualificationAsync(long sitterId, AddSitterQualificationDTO dto)
    {
        var user = await _context.Users.FindAsync(sitterId);
        if (user == null || !user.Role.HasFlag(UserRole.Sitter))
        {
            throw new UnauthorizedAccessException("User is not a sitter or not found.");
        }

        var qualificationType = await _context.QualificationTypes.FindAsync(dto.QualificationTypeId);
        if (qualificationType == null)
        {
            throw new KeyNotFoundException("Qualification type not found.");
        }

        if (await _context.SitterQualifications.AnyAsync(sq => sq.SitterId == sitterId && sq.QualificationTypeId == dto.QualificationTypeId))
        {
            throw new InvalidOperationException("Sitter already has this qualification.");
        }

        var sitterQualification = new SitterQualification
        {
            SitterId = sitterId,
            QualificationTypeId = dto.QualificationTypeId,
            GrantedAt = DateTime.UtcNow
        };

        _context.SitterQualifications.Add(sitterQualification);
        await _context.SaveChangesAsync();

        sitterQualification.QualificationType = qualificationType;

        return ToSitterQualificationDTO(sitterQualification);
    }

    public async Task<bool> RemoveSitterQualificationAsync(long sitterId, long qualificationTypeId)
    {
        var sitterQualification = await _context.SitterQualifications
            .FirstOrDefaultAsync(sq => sq.SitterId == sitterId && sq.QualificationTypeId == qualificationTypeId);

        if (sitterQualification == null)
        {
            return false;
        }

        _context.SitterQualifications.Remove(sitterQualification);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<SitterQualificationDTO>> GetSitterQualificationsAsync(long sitterId)
    {
        return await _context.SitterQualifications
            .Where(sq => sq.SitterId == sitterId)
            .Include(sq => sq.QualificationType)
            .Select(sq => ToSitterQualificationDTO(sq))
            .ToListAsync();
    }

    private static SitterQualificationDTO ToSitterQualificationDTO(SitterQualification sq)
    {
        return new SitterQualificationDTO
        {
            SitterQualificationId = sq.SitterQualificationId,
            SitterId = sq.SitterId,
            QualificationType = new QualificationTypeDTO
            {
                QualificationTypeId = sq.QualificationType.QualificationTypeId,
                Code = sq.QualificationType.Code,
                Name = sq.QualificationType.Code,
                Description = sq.QualificationType.Description
            },
            GrantedAt = sq.GrantedAt
        };
    }
    
}