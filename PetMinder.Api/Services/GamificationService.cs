using Microsoft.EntityFrameworkCore;
using PetMinder.Data;
using PetMinder.Models;
using PetMinder.Shared.DTO;

namespace PetMinder.Api.Services
{
    public interface IGamificationService
    {
        Task CheckAndAwardBadgesAsync(long userId);
        Task<List<BadgeDTO>> GetUserBadgesAsync(long userId);
    }

    public class GamificationService : IGamificationService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<GamificationService> _logger;

        public GamificationService(ApplicationDbContext context, ILogger<GamificationService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<BadgeDTO>> GetUserBadgesAsync(long userId)
        {
            return await _context.UserBadges
                .Where(ub => ub.UserId == userId)
                .Select(ub => new BadgeDTO
                {
                    BadgeId = ub.BadgeId,
                    Name = ub.Badge.Name,
                    Description = ub.Badge.Description,
                    AwardedAt = ub.AwardedAt
                })
                .ToListAsync();
        }

        public async Task CheckAndAwardBadgesAsync(long userId)
        {
            var user = await _context.Users
                .Include(u => u.SittingBookings)
                    .ThenInclude(b => b.Cancellation)
                .Include(u => u.SittingBookings)
                    .ThenInclude(b => b.Reviews)
                .Include(u => u.UserAddresses) // For completeness if needed later
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null) return;

            var badges = await _context.Badges.ToListAsync();
            var existingUserBadgeIds = await _context.UserBadges
                .Where(ub => ub.UserId == userId)
                .Select(ub => ub.BadgeId)
                .ToListAsync();

            foreach (var badge in badges)
            {
                if (existingUserBadgeIds.Contains(badge.BadgeId)) continue;

                bool isEarned = false;

                switch (badge.BadgeId)
                {
                    case 1: 
                        isEarned = CheckReliableSitterStreak(user);
                        break;
                    case 2: 
                        isEarned = CheckPeakSeasonHelper(user);
                        break;
                    case 3:
                        isEarned = CheckTopRated(user);
                        break;
                    case 4:
                        isEarned = CheckFrequentSitter(user);
                        break;
                    case 5:
                        isEarned = await CheckReviewer(userId);
                        break;
                }

                if (isEarned)
                {
                    _context.UserBadges.Add(new UserBadge
                    {
                        UserId = userId,
                        BadgeId = badge.BadgeId,
                        AwardedAt = DateTime.UtcNow
                    });
                    _logger.LogInformation($"Awarded badge {badge.Name} to user {userId}");
                }
            }

            await _context.SaveChangesAsync();
        }

        private bool CheckReliableSitterStreak(User user)
        {
            var completedBookings = user.SittingBookings
                .Where(b => b.Status == BookingStatus.Completed)
                .OrderByDescending(b => b.EndTime)
                .Take(5)
                .ToList();

            if (completedBookings.Count < 5) return false;
            
            var recentCancellations = _context.BookingCancellations
                .Any(bc => bc.BookingRequest.SitterId == user.UserId && bc.CancelledBy == CancelledBy.Sitter);

            return !recentCancellations && completedBookings.Count >= 5;
        }

        private bool CheckPeakSeasonHelper(User user)
        {
            var holidays = new List<(int Month, int Day)>
            {
                (12, 25), (12, 24), (12,31), (1, 1), (12, 31), (11, 28) //TODO: Add more holidays
            };

            return user.SittingBookings.Any(b =>
                b.Status == BookingStatus.Completed &&
                holidays.Any(h =>
                    (b.StartTime.Month == h.Month && b.StartTime.Day == h.Day) ||
                    (b.EndTime.Month == h.Month && b.EndTime.Day == h.Day) ||
                    (b.StartTime < new DateTime(b.StartTime.Year, h.Month, h.Day) && b.EndTime > new DateTime(b.StartTime.Year, h.Month, h.Day))
                )
            );
        }

        private bool CheckTopRated(User user)
        {
            var reviews = user.SittingBookings
                .SelectMany(b => b.Reviews)
                .Where(r => r.RevieweeId == user.UserId && r.SitterRating.HasValue)
                .ToList();

            if (reviews.Count < 10) return false;

            return reviews.Average(r => (double)r.SitterRating.Value) >= 4.8;
        }

        private bool CheckFrequentSitter(User user)
        {
            return user.SittingBookings.Count(b => b.Status == BookingStatus.Completed) >= 10;
        }

        private async Task<bool> CheckReviewer(long userId)
        {
            var count = await _context.Reviews.CountAsync(r => r.ReviewerId == userId);
            return count >= 5;
        }
    }
}
