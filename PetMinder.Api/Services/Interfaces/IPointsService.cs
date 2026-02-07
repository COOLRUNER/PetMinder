using PetMinder.Models;

namespace WebApplication1.Services.Interfaces;

public interface IPointsService
{
    Task<int> GetUserPointsBalanceAsync(long userId);
    Task<List<PetMinder.Shared.DTO.PointsLotDTO>> GetUserPointsLotsAsync(long userId);
    Task DeductPointsAsync(long senderId, long receiverId, int points, TransactionType type, string reason);

    Task CreditPointsAsync(long receiverId, long senderId, int points, TransactionType type, string reason);
    Task ExpirePointsAsync();


}