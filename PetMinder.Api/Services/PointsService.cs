using System.Transactions;
using Microsoft.EntityFrameworkCore;
using PetMinder.Data;
using PetMinder.Models;
using WebApplication1.Services.Interfaces;

namespace WebApplication1.Services;

public class PointsService : IPointsService
{
    private readonly ApplicationDbContext _context;

    public PointsService(ApplicationDbContext context)
    {
        _context = context;
    }

    private async Task<long> GetPointPolicyIdForTransaction(TransactionType type, PointPolicyServiceType? serviceType = null)
    {
        PointPolicy policy = null;

        if (type == TransactionType.Booking)
        {
            policy = await _context.PointPolicies
                            .FirstOrDefaultAsync(p => p.ServiceType == PointPolicyServiceType.Pet);
        }

        if (policy == null)
        {
            policy = await _context.PointPolicies
                            .FirstOrDefaultAsync(p => p.ServiceType == PointPolicyServiceType.Pet);
            if (policy == null)
            {
                throw new InvalidOperationException("No suitable PointPolicy found for this transaction type. " +
                                                    "Please ensure a PointPolicy for 'Pet' service type is seeded in your database.");
            }
        }

        return policy.PolicyId;
    }

    public async Task<int> GetUserPointsBalanceAsync(long userId)
    {
        var balance = await _context.PointsLots
            .Where(l => l.UserId == userId && !l.IsExpired && l.PointsRemaining > 0)
            .SumAsync(l => l.PointsRemaining);
        return balance;
    }

    public async Task<List<PetMinder.Shared.DTO.PointsLotDTO>> GetUserPointsLotsAsync(long userId)
    {
        return await _context.PointsLots
            .AsNoTracking()
            .Include(l => l.PointsTransaction)
            .Where(l => l.UserId == userId && l.PointsRemaining > 0 && !l.IsExpired)
            .OrderBy(l => l.ExpiresAt ?? DateTime.MaxValue)
            .Select(l => new PetMinder.Shared.DTO.PointsLotDTO
            {
                LotId = l.LotId,
                PointsIssued = l.PointsIssued,
                PointsRemaining = l.PointsRemaining,
                CreatedAt = l.CreatedAt,
                ExpiresAt = l.ExpiresAt,
                IsExpired = l.IsExpired,
                TransactionReason = l.PointsTransaction.Reason
            })
            .ToListAsync();
    }


    public async Task DeductPointsAsync(long senderId, long receiverId, int points, TransactionType type, string reason)
    {
        if (points <= 0) throw new ArgumentOutOfRangeException(nameof(points), "Points to deduct must be positive.");
        if (string.IsNullOrWhiteSpace(reason)) throw new ArgumentException("Reason is required for the transaction.", nameof(reason));

        var available = await GetUserPointsBalanceAsync(senderId);
        if (available < points)
        {
            throw new InvalidOperationException($"Insufficient points balance for user ID {senderId}. Current: {available}, Needed: {points}.");
        }

        var policyId = await GetPointPolicyIdForTransaction(type);

        try
        {
            var now = DateTime.UtcNow;
            var lots = await _context.PointsLots
                .Where(l => l.UserId == senderId && !l.IsExpired && l.PointsRemaining > 0 && (l.ExpiresAt == null || l.ExpiresAt > now))
                .OrderBy(l => l.ExpiresAt ?? DateTime.MaxValue)
                .ToListAsync();

            int pointsToDeduct = points;
            foreach (var lot in lots)
            {
                if (pointsToDeduct <= 0) break;

                int take = Math.Min(lot.PointsRemaining, pointsToDeduct);
                lot.PointsRemaining -= take;
                pointsToDeduct -= take;

                if (lot.PointsRemaining == 0 && lot.ExpiresAt != null && lot.ExpiresAt <= now)
                {
                    lot.IsExpired = true;
                }
                _context.PointsLots.Update(lot);
            }

            if (pointsToDeduct > 0)
            {
                throw new InvalidOperationException("Not enough points in lots to deduct the required amount.");
            }

            var transaction = new PointsTransaction
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Points = points,
                TransactionType = type,
                OccurredAt = DateTime.UtcNow,
                Reason = reason,
                PolicyId = policyId
            };
            _context.PointsTransactions.Add(transaction);
            await _context.SaveChangesAsync();
        }
        catch
        {
            throw;
        }

    }

    public async Task CreditPointsAsync(long receiverId, long senderId, int points, TransactionType type, string reason)
    {
        if (points <= 0) throw new ArgumentOutOfRangeException(nameof(points), "Points to credit must be positive.");
        if (string.IsNullOrWhiteSpace(reason)) throw new ArgumentException("Reason is required for the transaction.", nameof(reason));

        var policyId = await GetPointPolicyIdForTransaction(type);

        try
        {
            var transaction = new PointsTransaction
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Points = points,
                TransactionType = type,
                OccurredAt = DateTime.UtcNow,
                Reason = reason,
                PolicyId = policyId
            };
            _context.PointsTransactions.Add(transaction);
            await _context.SaveChangesAsync();


            var policy = await _context.PointPolicies.FindAsync(policyId)
                         ?? throw new InvalidOperationException("PointPolicy not found.");

            var pointsLot = new PointsLot
            {
                UserId = receiverId,
                TransactionId = transaction.TrasactionId,
                PointsIssued = points,
                PointsRemaining = points,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = policy.ExpiryDays != 0 ? DateTime.UtcNow.AddDays(policy.ExpiryDays) : (DateTime?)null,
                IsExpired = false
            };
            _context.PointsLots.Add(pointsLot);
            await _context.SaveChangesAsync();
        }
        catch
        {
            throw;
        }
    }

    public async Task ExpirePointsAsync() 
    {
        var now = DateTime.UtcNow;
        var lotsToExpire = await _context.PointsLots
            .Where(l => !l.IsExpired && l.ExpiresAt != null && l.ExpiresAt <= now && l.PointsRemaining > 0)
            .Include(pointsLot => pointsLot.PointsTransaction)
            .ToListAsync();

        if (!lotsToExpire.Any()) return;

        using var tx = await _context.Database.BeginTransactionAsync();

        try
        {
            foreach (var lot in lotsToExpire)
            {
                var ExpiredPoints = lot.PointsRemaining;
                lot.IsExpired = true;
                lot.PointsRemaining = 0;
                _context.PointsLots.Update(lot);

                var expiryTransaction = new PointsTransaction()
                {
                    OccurredAt = now,
                    ReceiverId = -1, 
                    SenderId = lot.UserId,
                    Points = ExpiredPoints,
                    TransactionType = TransactionType.Expiration,
                    Reason = $"Expiration of {ExpiredPoints} points from lot {lot.LotId}",
                    PolicyId = ((await _context.PointPolicies.FindAsync(lot.PointsTransaction.PolicyId))).PolicyId
                };
                _context.PointsTransactions.Add(expiryTransaction);
            }
            await _context.SaveChangesAsync();
            await tx.CommitAsync();
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }
    }


}