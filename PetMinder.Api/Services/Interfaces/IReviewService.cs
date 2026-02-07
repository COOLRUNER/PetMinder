using PetMinder.Models;
using PetMinder.Shared.DTO;

namespace WebApplication1.Services.Interfaces;

public interface IReviewService
{
    Task<Review> SubmitReviewAsync(long userId, CreateReviewDTO createReviewDto);

    Task<UserAggregatedRatingDTO> GetAggregatedRatingsAsync(long userId);
    
    Task<List<ReviewDTO>> GetRecentReviewsByRevieweeAsync(long revieweeId, int count = 5);
}