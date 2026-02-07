using Microsoft.EntityFrameworkCore;
using PetMinder.Data;
using PetMinder.Models;
using WebApplication1.Services.Interfaces;

namespace WebApplication1.Services;

public class VerificationService : IVerificationService
{
    private readonly ApplicationDbContext _context;
    private readonly IPointsService _pointsService;
    private readonly ILogger<VerificationService> _logger;


    public VerificationService(ApplicationDbContext context, IPointsService pointsService, ILogger<VerificationService> logger)
    {
        _context = context;
        _pointsService = pointsService;
        _logger = logger;
    }

    public async Task CompleteVerificationStep(long userId, VerificationStep step)
    {
        if (await _context.UserVerificationSteps.AnyAsync(v => v.UserId == userId && v.Step == step))
        {
            return;
        }
        
        int points = step switch
        {
            VerificationStep.EmailVerification => 50,
            VerificationStep.ProfilePhotoUpload => 50,
            _ => 0
        };

        var verificationStep = new UserVerificationStep
        {
            UserId = userId,
            Step = step,
            CompletedAt = DateTime.UtcNow,
            PointsAwarded = points
        };
        _context.UserVerificationSteps.Add(verificationStep);

        if (points > 0)
        {
            try
            {
                const long systemUserId = -1;
                await _pointsService.CreditPointsAsync(
                    receiverId: userId,
                    senderId: systemUserId,
                    points: points,
                    type: TransactionType.VerificationReward,
                    reason: $"reward for {step} completion"
                );
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to award points for verification step {Step} for user with id: {UserId}", step, userId);
            }
        }

        await _context.SaveChangesAsync();

        if (await IsFullyVerified(userId))
        {
            var user = await _context.Users.Include(u => u.SitterSettings).FirstOrDefaultAsync(u => u.UserId == userId);
            if (user != null)
            {
                bool changed = false;
                if (!user.Role.HasFlag(UserRole.Owner))
                {
                    user.Role |= UserRole.Owner;
                    changed = true;
                }
                if (!user.Role.HasFlag(UserRole.Sitter))
                {
                    user.Role |= UserRole.Sitter;
                    changed = true;
                    
                    if (user.SitterSettings == null)
                    {
                        user.SitterSettings = new SitterSettings { MinPoints = 0 };
                    }
                }

                if (changed)
                {
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("User {UserId} auto-granted Owner/Sitter roles after full verification.", userId);
                }
            }
        }
    }

    public async Task<bool> IsVerificationStepComplete(long userId, VerificationStep step)
    {
        return await _context.UserVerificationSteps.AnyAsync(v => v.UserId == userId && v.Step == step);
    }
    

    public async Task<List<VerificationStep>> GetCompletedSteps(long userId)
    {
        return await _context.UserVerificationSteps
            .Where(uvs => uvs.UserId == userId)
            .Select(uvs => uvs.Step)
            .ToListAsync();
    }

    public async Task<bool> IsFullyVerified(long userId)
    {
        var requiredSteps = new[]
        {
            VerificationStep.EmailVerification,
            VerificationStep.ProfilePhotoUpload
        };

        foreach (var step in requiredSteps)
        {
            if (!await IsVerificationStepComplete(userId, step))
            {
                return false;
            }
        }

        return true;
    }
}