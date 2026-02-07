using Microsoft.EntityFrameworkCore;
using PetMinder.Data;
using PetMinder.Models;
using PetMinder.Shared.DTO;
using WebApplication1.Services.Interfaces;

using PetMinder.Api.Services;

namespace WebApplication1.Services;

public class ReviewService : IReviewService
{
    private readonly ApplicationDbContext _context;
    private readonly IGamificationService _gamificationService;

    public ReviewService(ApplicationDbContext context, IGamificationService gamificationService)
    {
        _context = context;
        _gamificationService = gamificationService;
    }

    public async Task<Review> SubmitReviewAsync(long userId, CreateReviewDTO createReviewDto)
    {
        var booking = await _context.BookingRequests
            .Include(b => b.Reviews)
            .FirstOrDefaultAsync(b => b.BookingId == createReviewDto.BookingId);

        if (booking == null)
        {
            throw new KeyNotFoundException($"Booking {createReviewDto.BookingId} not found.");
        }

        if (booking.Status != BookingStatus.Completed)
        {
            throw new InvalidOperationException("Booking must be completed to be able to leave a review.");
        }

        long revieweeId;
        string requiredRatingType;
        string requiredRole;

        if (userId == booking.OwnerId)
        {
            revieweeId = booking.SitterId;
            requiredRatingType = nameof(Review.OwnerRating);
            requiredRole = nameof(UserRole.Owner);
        }
        else if (userId == booking.SitterId)
        {
            revieweeId = booking.OwnerId;
            requiredRatingType = nameof(Review.SitterRating);
            requiredRole = nameof(UserRole.Sitter);
        }
        else
        {
            throw new UnauthorizedAccessException("Reviewer is not participating in this booking.");
        }

        if (booking.Reviews.Any(r => r.ReviewerId == userId))
        {
            throw new InvalidOperationException($"This user already submitted their review " +
                                                $"for booking {booking.BookingId} by {requiredRole}");
        }

        var review = new Review
        {
            ReviewerId = userId,
            RevieweeId = revieweeId,
            BookingId = booking.BookingId,
            Comment = createReviewDto.Comment,
            CreatedAt = DateTime.UtcNow
        };

        if (requiredRole == nameof(UserRole.Owner))
        {
            if (!createReviewDto.SitterRating.HasValue)
            {
                throw new ArgumentException("Owner must provide a sitter rating.");
            }

            review.SitterRating = createReviewDto.SitterRating;
        }

        if (requiredRole == nameof(UserRole.Sitter))
        {
            if (!createReviewDto.OwnerRating.HasValue || !createReviewDto.PetRating.HasValue)
            {
                throw new ArgumentException("Sitter must provide an owner and a pet rating.");
            }

            review.OwnerRating = createReviewDto.OwnerRating;
            review.PetRating = createReviewDto.PetRating;

            review.PetId = booking.PetId;

            if (createReviewDto.HouseRating.HasValue)
            {
                review.HouseRating = createReviewDto.HouseRating;
            }
        }

        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();

        await _gamificationService.CheckAndAwardBadgesAsync(review.ReviewerId); // Reviewer
        await _gamificationService.CheckAndAwardBadgesAsync(review.RevieweeId); // Top Rated etc.

        return review;
    }

    public async Task<UserAggregatedRatingDTO> GetAggregatedRatingsAsync(long userId)
    {
        var reviews = await _context.Reviews
            .AsNoTracking()
            .Where(r => r.RevieweeId == userId)
            .ToListAsync();

        var sitterReviews = reviews.Where(r => r.SitterRating.HasValue).ToList();
        var ownerReviews = reviews.Where(r => r.OwnerRating.HasValue).ToList();

        double avgSitter =
            sitterReviews.Any()
                ? Math.Round(sitterReviews.Average(r => (double)r.SitterRating.Value), 2)
                : 0.0;

        double avgOwner = ownerReviews.Any()
            ? Math.Round(ownerReviews.Average(r => (double)r.OwnerRating.Value), 2)
            : 0.0;

        bool isTrustedSitter = sitterReviews.Count(r => r.SitterRating >= 4) >= 5;

        return new UserAggregatedRatingDTO
        {
            AverageSitterRating = avgSitter,
            AverageOwnerRating = avgOwner,
            SitterReviewCount = sitterReviews.Count,
            OwnerReviewCount = ownerReviews.Count,
            IsTrustedSitter = isTrustedSitter
        };
    }

    public async Task<List<ReviewDTO>> GetRecentReviewsByRevieweeAsync(long revieweeId, int count = 5)
    {
        return await _context.Reviews
            .AsNoTracking()
            .Where(r => r.RevieweeId == revieweeId)
            .OrderByDescending(r => r.CreatedAt)
            .Take(count)
            .Include(r => r.Reviewer)
            .Select(r => new ReviewDTO
            {
                ReviewId = r.ReviewId,
                ReviewerId = r.ReviewerId,
                ReviewerName = r.Reviewer.FirstName + " " + r.Reviewer.LastName,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt,
                BookingId = r.BookingId,
                HouseRating = r.HouseRating,
                SitterRating = r.SitterRating,
                OwnerRating = r.OwnerRating,
                PetRating = r.PetRating
            })
            .ToListAsync();
    }
}