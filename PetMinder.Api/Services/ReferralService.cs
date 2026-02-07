using Microsoft.EntityFrameworkCore;
using PetMinder.Data;
using PetMinder.Models;
using PetMinder.Shared.DTO;
using WebApplication1.Services.Interfaces;

namespace PetMinder.Api.Services
{
    public interface IReferralService
    {
        Task<string> GetOrCreateReferralCodeAsync(long userId);
        Task ApplyReferralAsync(string code, long newUserId);
        Task CompleteReferralAsync(long referredUserId);
    }

    public class ReferralService : IReferralService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ReferralService> _logger;
        private readonly IPointsService _pointsService;
        private readonly INotificationService _notificationService;

        public ReferralService(ApplicationDbContext context, ILogger<ReferralService> logger, IPointsService pointsService, INotificationService notificationService)
        {
            _context = context;
            _logger = logger;
            _pointsService = pointsService;
            _notificationService = notificationService;
        }

        public async Task<string> GetOrCreateReferralCodeAsync(long userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) throw new ArgumentException("User not found");

            if (!string.IsNullOrEmpty(user.ReferralCode))
            {
                return user.ReferralCode;
            }

            string code;
            do
            {
                code = GenerateReferralCode(user.FirstName);
            } while (await _context.Users.AnyAsync(u => u.ReferralCode == code));

            user.ReferralCode = code;
            await _context.SaveChangesAsync();

            return code;
        }

        public async Task ApplyReferralAsync(string code, long newUserId)
        {
            var referrer = await _context.Users.FirstOrDefaultAsync(u => u.ReferralCode == code);
            if (referrer == null)
            {
                _logger.LogWarning($"Invalid referral code used: {code}");
                return;
            }

            if (referrer.UserId == newUserId)
            {
                _logger.LogWarning($"User tried to refer themselves: {newUserId}");
                return;
            }

            var existingReferral = await _context.Referrals.FirstOrDefaultAsync(r => r.ReferredUserId == newUserId);
            if (existingReferral != null)
            {
                _logger.LogWarning($"User {newUserId} already referred.");
                return;
            }

            var referral = new Referral
            {
                ReferrerId = referrer.UserId,
                ReferredUserId = newUserId,
                Code = code,
                Status = ReferralStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            _context.Referrals.Add(referral);
            await _context.SaveChangesAsync();
        }

        public async Task CompleteReferralAsync(long referredUserId)
        {
            var refferalPoints = 500;
            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                var referral = await _context.Referrals
                    .Include(r => r.ReferredUser)
                    .FirstOrDefaultAsync(r => r.ReferredUserId == referredUserId && r.Status == ReferralStatus.Pending);

                if (referral == null) return;

                referral.Status = ReferralStatus.Completed;
                referral.CompletedAt = DateTime.UtcNow;

                await _pointsService.CreditPointsAsync(
                    receiverId: referral.ReferrerId,
                    senderId: -1, // System
                    points: refferalPoints,
                    type: TransactionType.ReferralBonus,
                    reason: $"Referral bonus for referring user {referredUserId}"
                );
                _logger.LogInformation($"Referral completed for {referredUserId}. Referrer {referral.ReferrerId} awarded points.");
                
                await _context.SaveChangesAsync();
                await tx.CommitAsync();
                
                
                await _notificationService.CreateNotificationAsync(
                    referral.ReferrerId,
                    NotificationType.SystemAlert,
                    $"You have earned {refferalPoints} points for referring {referral.ReferredUser.FirstName}!"
                );
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                _logger.LogError(ex, $"Error completing referral for {referredUserId}");
                throw;
            }
        }

        private string GenerateReferralCode(string firstName)
        {
            var random = new Random();
            var suffix = random.Next(1000, 9999);
            var cleanName = new string(firstName.Where(char.IsLetterOrDigit).ToArray()).ToUpper();
            if (cleanName.Length > 4) cleanName = cleanName.Substring(0, 4);
            return $"{cleanName}{suffix}";
        }
    }
}
