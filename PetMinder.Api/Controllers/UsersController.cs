using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetMinder.Shared.DTO;
using WebApplication1.Services.Interfaces;
using PetMinder.Data;
using PetMinder.Models;
using Microsoft.EntityFrameworkCore;
using PetMinder.Api.Services;



namespace WebApplication1.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly PetMinder.Data.ApplicationDbContext _context;
    private readonly IPointsService _pointsService;
    private readonly IReviewService _reviewService;
    private readonly IGamificationService _gamificationService;
    private readonly IReferralService _referralService;
    private readonly IVerificationService _verificationService;

    public UsersController(IAuthService authService, PetMinder.Data.ApplicationDbContext context,
        IPointsService pointsService, IReviewService reviewService, IGamificationService gamificationService, IReferralService referralService, IVerificationService verificationService)
    {
        _authService = authService;
        _context = context;
        _pointsService = pointsService;
        _reviewService = reviewService;
        _gamificationService = gamificationService;
        _referralService = referralService;
        _verificationService = verificationService;
    }

    private long GetUserId() => long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

    [HttpGet("me/points")]
    public async Task<ActionResult<int>> GetMyPoints()
    {
        var userId = GetUserId();
        var points = await _pointsService.GetUserPointsBalanceAsync(userId);
        return Ok(points);
    }

    [HttpGet("me/points/lots")]
    public async Task<ActionResult<IEnumerable<PointsLotDTO>>> GetMyPointsLots()
    {
        var userId = GetUserId();
        var pointsLots = await _pointsService.GetUserPointsLotsAsync(userId);
        return Ok(pointsLots);
    }

    [HttpGet("me/transactions")]
    public async Task<ActionResult<IEnumerable<TransactionDTO>>> GetMyTransactions()
    {
        var userId = GetUserId();

        var transactions = await _context.PointsTransactions
            .AsNoTracking()
            .Include(t => t.Sender)
            .Include(t => t.Receiver)
            .Include(t => t.PointPolicy)
            .Where(t => t.SenderId == userId || t.ReceiverId == userId)
            .OrderByDescending(t => t.OccurredAt)
            .Select(t => new TransactionDTO
            {
                TransactionId = t.TrasactionId,
                SenderId = t.SenderId,
                SenderName = t.Sender.FirstName + " " + t.Sender.LastName,
                ReceiverId = t.ReceiverId,
                ReceiverName = t.Receiver.FirstName + " " + t.Receiver.LastName,
                Points = t.Points,
                TransactionType = t.TransactionType.ToString(),
                OccurredAt = t.OccurredAt,
                Reason = t.Reason,
                PolicyId = t.PolicyId,
                ExpiresAt = (t.PointPolicy != null && t.PointPolicy.ExpiryDays != 0)
                    ? (DateTime?)t.OccurredAt.AddDays(t.PointPolicy.ExpiryDays)
                    : null
            })
            .ToListAsync();

        return Ok(transactions);
    }

    [HttpPut("me")]
    public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateUserDTO dto)
    {
        var userId = GetUserId();
        try
        {
            var result = await _authService.UpdateUserProfileAsync(userId, dto);
            if (!result)
            {
                return NotFound("User not found.");
            }
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error updating user profile.", details = ex.Message });
        }
    }

    [HttpGet("me")]
    public async Task<ActionResult<UserProfileDTO>> GetMyProfile()
    {
        var userId = GetUserId();
        var user = await _context.Users.AsNoTracking().Include(u => u.SitterSettings).Include(u => u.ReferralsMade).FirstOrDefaultAsync(u => u.UserId == userId);
        if (user == null)
        {
            return NotFound();
        }
        var aggregatedRatings = await _reviewService.GetAggregatedRatingsAsync(userId);
        var recentReviews = await _reviewService.GetRecentReviewsByRevieweeAsync(userId, 10); // Get up to 10 recent reviews
        
        var referralCode = await _referralService.GetOrCreateReferralCodeAsync(userId);
        
        var dto = new UserProfileDTO
        {
            UserId = user.UserId,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Phone = user.Phone,
            ProfilePhotoUrl = user.ProfilePhotoUrl,
            Roles = System.Enum.GetValues(typeof(UserRole))
                .Cast<UserRole>()
                .Where(r => r != UserRole.None && user.Role.HasFlag(r))
                .Select(r => r.ToString())
                .ToArray(),

            AverageRating = aggregatedRatings.AverageSitterRating,
            ReviewCount = aggregatedRatings.SitterReviewCount,
            AverageOwnerRating = aggregatedRatings.AverageOwnerRating,
            OwnerReviewCount = aggregatedRatings.OwnerReviewCount,
            RecentReviews = recentReviews,

            MinPoints = user.SitterSettings?.MinPoints ?? 0,
            Badges = await _gamificationService.GetUserBadgesAsync(userId),
            ReferralCode = referralCode,
            ReferralsCount = user.ReferralsMade?.Count ?? 0,
            IsVerified = await _verificationService.IsFullyVerified(userId)
        };
        return Ok(dto);
    }

    [HttpGet("{userId}/profile")]
    public async Task<ActionResult<SearchSitterProfileDetailsDTO>> GetUserProfile(long userId)
    {
        var aggregatedRatings = await _reviewService.GetAggregatedRatingsAsync(userId);
        var recentReviews = await _reviewService.GetRecentReviewsByRevieweeAsync(userId);


        var baseUserQuery = await _context.Users
            .AsNoTracking()
            .Include(u => u.SitterSettings)
            .Include(u => u.Availabilities)
            .Include(u => u.SitterQualifications)
            .ThenInclude(sq => sq.QualificationType)
            .Include(u => u.SitterRestrictions)
            .ThenInclude(sr => sr.RestrictionType)
            .Where(u => u.UserId == userId)
            .FirstOrDefaultAsync();

        if (baseUserQuery == null)
        {
            return NotFound();
        }

        var dto = new SearchSitterProfileDetailsDTO
        {
            UserId = baseUserQuery.UserId,
            Email = baseUserQuery.Email,
            FirstName = baseUserQuery.FirstName,
            LastName = baseUserQuery.LastName,
            Phone = baseUserQuery.Phone,
            ProfilePhotoUrl = baseUserQuery.ProfilePhotoUrl,
            Roles = System.Enum.GetValues(typeof(UserRole))
                .Cast<UserRole>()
                .Where(r => r != UserRole.None && baseUserQuery.Role.HasFlag(r))
                .Select(r => r.ToString())
                .ToArray(),

            MinPoints = baseUserQuery.SitterSettings?.MinPoints ?? 0,
            Availabilities = baseUserQuery.Availabilities
                .Select(a => new SitterAvailabilityDTO
                {
                    AvailabilityId = a.AvailabilityId,
                    SitterId = a.SitterId,
                    StartTime = a.StartTime,
                    EndTime = a.EndTime
                }).ToList(),
            Qualifications = baseUserQuery.SitterQualifications
                .Select(sq => new SitterQualificationDTO
                {
                    SitterQualificationId = sq.SitterQualificationId,
                    SitterId = sq.SitterId,
                    QualificationType = new QualificationTypeDTO
                    {
                        QualificationTypeId = sq.QualificationType.QualificationTypeId,
                        Code = sq.QualificationType.Code,
                        Description = sq.QualificationType.Description
                    },
                    GrantedAt = sq.GrantedAt
                }).ToList(),
            Restrictions = baseUserQuery.SitterRestrictions
                .Select(sr => new SitterRestrictionDTO
                {
                    SitterRestrictionId = sr.SitterRestrictionId,
                    SitterId = sr.SitterId,
                    RestrictionType = new RestrictionTypeDTO
                    {
                        RestrictionTypeId = sr.RestrictionType.RestrictionTypeId,
                        Code = sr.RestrictionType.Code,
                        Description = sr.RestrictionType.Description
                    },
                    SetAt = sr.SetAt
                }).ToList(),

            AverageSitterRating = aggregatedRatings.AverageSitterRating,
            SitterReviewCount = aggregatedRatings.SitterReviewCount,
            AverageOwnerRating = aggregatedRatings.AverageOwnerRating,
            OwnerReviewCount = aggregatedRatings.OwnerReviewCount,

            RecentReviews = recentReviews,
            Badges = await _gamificationService.GetUserBadgesAsync(userId),
            IsVerified = await _verificationService.IsFullyVerified(userId)
        };

        return Ok(dto);
    }
}