using Microsoft.EntityFrameworkCore;
using PetMinder.Data;
using PetMinder.Models;
using PetMinder.Shared.DTO;
using WebApplication1.Services.Interfaces;
using WebApplication1.Utils;

namespace WebApplication1.Services;

public class SitterAvailabilityService : ISitterAvailabilityService
{
    private readonly ApplicationDbContext _context;

    public SitterAvailabilityService(ApplicationDbContext context)
    {
        _context = context;
    }

    private static SitterAvailabilityDTO ToSitterAvailabilityDTO(SitterAvailability sa)
    {
        return new SitterAvailabilityDTO
        {
            AvailabilityId = sa.AvailabilityId,
            SitterId = sa.SitterId,
            StartTime = sa.StartTime,
            EndTime = sa.EndTime
        };
    }

    public async Task<SitterAvailabilityDTO> AddSitterAvailabilityAsync(long sitterId, AddSitterAvailabilityDTO dto)
    {
        if (dto.StartTime >= dto.EndTime)
        {
            throw new ArgumentException("End time must be after start time.");
        }

        bool hasOverlap = await _context.SitterAvailabilities.AnyAsync(sa =>
            sa.SitterId == sitterId &&
            sa.StartTime < dto.EndTime &&
            sa.EndTime > dto.StartTime);

        if (hasOverlap)
        {
            throw new InvalidOperationException("New availability overlaps with an existing time slot.");
        }

        var sitterAvailability = new SitterAvailability
        {
            SitterId = sitterId,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime
        };

        _context.SitterAvailabilities.Add(sitterAvailability);
        await _context.SaveChangesAsync();

        return ToSitterAvailabilityDTO(sitterAvailability);
    }

    public async Task<List<SitterAvailabilityDTO>> GetSitterAvailabilitiesAsync(long sitterId)
    {
        return await _context.SitterAvailabilities
            .Where(sa => sa.SitterId == sitterId)
            .OrderBy(sa => sa.StartTime)
            .Select(sa => ToSitterAvailabilityDTO(sa))
            .ToListAsync();
    }

    public async Task<SitterAvailabilityDTO> GetSitterAvailabilityByIdAsync(long availabilityId)
    {
        var availability = await _context.SitterAvailabilities.FindAsync(availabilityId);
        return availability != null ? ToSitterAvailabilityDTO(availability) : null;
    }

    public async Task<bool> UpdateSitterAvailabilityAsync(long sitterId, long availabilityId, UpdateSitterAvailabilityDTO dto)
    {
        var existingAvailability = await _context.SitterAvailabilities.FirstOrDefaultAsync(sa => sa.AvailabilityId == availabilityId && sa.SitterId == sitterId);
        if (existingAvailability == null)
        {
            return false;
        }

        DateTime newStartTime = dto.StartTime;
        DateTime newEndTime = dto.EndTime;

        if (newStartTime >= newEndTime)
        {
            throw new ArgumentException("End time must be after start time.");
        }

        bool hasOverlap = await _context.SitterAvailabilities.AnyAsync(sa =>
            sa.SitterId == sitterId &&
            sa.AvailabilityId != availabilityId &&
            sa.StartTime < newEndTime &&
            sa.EndTime > newStartTime);

        if (hasOverlap)
        {
            throw new InvalidOperationException("Updated availability overlaps with an existing time slot.");
        }

        existingAvailability.StartTime = newStartTime;
        existingAvailability.EndTime = newEndTime;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteSitterAvailabilityAsync(long sitterId, long availabilityId)
    {
        var availability = await _context.SitterAvailabilities.FirstOrDefaultAsync(sa => sa.AvailabilityId == availabilityId && sa.SitterId == sitterId);
        if (availability == null)
        {
            return false;
        }

        _context.SitterAvailabilities.Remove(availability);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<UserProfileDTO>> FindAvailableSittersAsync(SitterSearchRequestDTO searchRequest, long ownerId)
    {
        var utcDesiredStartTime = searchRequest.DesiredStartTime.ToUniversalTime();
        var utcDesiredEndTime = searchRequest.DesiredEndTime.ToUniversalTime();

        if (utcDesiredStartTime >= utcDesiredEndTime)
        {
            throw new ArgumentException("End time must be after start time for the search query.");
        }

        var ownerAddress = await _context.Addresses
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.AddressId == searchRequest.SearchFromAddressId);

        if (ownerAddress != null)
        {
            bool addressBelongsToOwner = await _context.UserAddresses
                .AnyAsync(ua => ua.AddressId == searchRequest.SearchFromAddressId && ua.UserId == ownerId);

            if (!addressBelongsToOwner)
            {
                ownerAddress = null;
            }
        }

        var availableSitterUserIds = await _context.SitterAvailabilities
            .Where(sa =>
                sa.StartTime <= utcDesiredEndTime &&
                sa.EndTime >= utcDesiredStartTime
            )
            .Select(sa => sa.SitterId)
            .Distinct()
            .ToListAsync();

        if (!availableSitterUserIds.Any())
        {
            return new List<UserProfileDTO>();
        }

        var availableSittersProfiles = await _context.Users
            .Where(u => availableSitterUserIds.Contains(u.UserId))
            .Select(u => new
            {
                User = u,
                SitterAddress = u.UserAddresses
                                 .Where(ua => ua.Type == AddressType.Primary)
                                 .Select(ua => new { ua.Address.Latitude, ua.Address.Longitude })
                                 .FirstOrDefault()
            })
            .Select(data => new UserProfileDTO
            {
                UserId = data.User.UserId,
                Email = data.User.Email,
                FirstName = data.User.FirstName,
                LastName = data.User.LastName,
                Phone = data.User.Phone,
                ProfilePhotoUrl = data.User.ProfilePhotoUrl,
                Roles = data.User.Role.ToString().Contains("Sitter") ? new string[] { "Sitter" } : System.Array.Empty<string>(),
                DistanceInKm = (ownerAddress != null && data.SitterAddress != null)
                    ? GeoUtils.Calculate(
                          ownerAddress.Latitude,
                          ownerAddress.Longitude,
                          data.SitterAddress.Latitude,
                          data.SitterAddress.Longitude
                      )
                    : (double?)null,
                ReviewCount = data.User.ReviewsReceived.Count(r => r.SitterRating.HasValue),
                AverageRating = Math.Round(
                    data.User.ReviewsReceived
                        .Where(r => r.SitterRating.HasValue)
                        .Average(r => (double?)r.SitterRating) ?? 0.0,
                    1)
            })
            .ToListAsync();

        return availableSittersProfiles
            .OrderBy(s => s.DistanceInKm.HasValue ? 0 : 1)
            .ThenBy(s => s.DistanceInKm)
            .ToList();
    }
}