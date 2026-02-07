using Microsoft.EntityFrameworkCore;
using PetMinder.Data;
using PetMinder.Models;
using PetMinder.Shared.DTO;
using WebApplication1.Services.Interfaces;

namespace WebApplication1.Services;

public class RestrictionService : IRestrictionService
{
    private readonly ApplicationDbContext _context;

        public RestrictionService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<RestrictionTypeDTO>> GetAllRestrictionTypesAsync()
        {
            return await _context.RestrictionTypes
                .Select(rt => new RestrictionTypeDTO
                {
                    RestrictionTypeId = rt.RestrictionTypeId,
                    Code = rt.Code,
                    Name = rt.Code,
                    Description = rt.Description
                })
                .ToListAsync();
        }

        public async Task<SitterRestrictionDTO> AddSitterRestrictionAsync(long sitterId, AddSitterRestrictionDTO dto)
        {
            var user = await _context.Users.FindAsync(sitterId);
            if (user == null || !user.Role.HasFlag(UserRole.Sitter))
            {
                throw new UnauthorizedAccessException("User is not a sitter or not found.");
            }

            var restrictionType = await _context.RestrictionTypes.FindAsync(dto.RestrictionTypeId);
            if (restrictionType == null)
            {
                throw new KeyNotFoundException("Restriction type not found.");
            }

            if (await _context.SitterRestrictions.AnyAsync(sr => sr.SitterId == sitterId && sr.RestrictionTypeId == dto.RestrictionTypeId))
            {
                throw new InvalidOperationException("Sitter already has this restriction.");
            }

            var sitterRestriction = new SitterRestriction
            {
                SitterId = sitterId,
                RestrictionTypeId = dto.RestrictionTypeId,
                SetAt = DateTime.UtcNow
            };

            _context.SitterRestrictions.Add(sitterRestriction);
            await _context.SaveChangesAsync();

            sitterRestriction.RestrictionType = restrictionType;

            return ToSitterRestrictionDTO(sitterRestriction);
        }

        public async Task<bool> RemoveSitterRestrictionAsync(long sitterId, long restrictionTypeId)
        {
            var sitterRestriction = await _context.SitterRestrictions
                .FirstOrDefaultAsync(sr => sr.SitterId == sitterId && sr.RestrictionTypeId == restrictionTypeId);

            if (sitterRestriction == null)
            {
                return false;
            }

            _context.SitterRestrictions.Remove(sitterRestriction);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<SitterRestrictionDTO>> GetSitterRestrictionsAsync(long sitterId)
        {
            return await _context.SitterRestrictions
                .Where(sr => sr.SitterId == sitterId)
                .Include(sr => sr.RestrictionType)
                .Select(sr => ToSitterRestrictionDTO(sr))
                .ToListAsync();
        }

        private static SitterRestrictionDTO ToSitterRestrictionDTO(SitterRestriction sr)
        {
            return new SitterRestrictionDTO
            {
                SitterRestrictionId = sr.SitterRestrictionId,
                SitterId = sr.SitterId,
                RestrictionType = new RestrictionTypeDTO
                {
                    RestrictionTypeId = sr.RestrictionType.RestrictionTypeId,
                    Code = sr.RestrictionType.Code,
                    Name = sr.RestrictionType.Code,
                    Description = sr.RestrictionType.Description
                },
                SetAt = sr.SetAt
            };
        }
}