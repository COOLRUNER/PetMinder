using Microsoft.EntityFrameworkCore;
using PetMinder.Data;
using PetMinder.Models;
using PetMinder.Shared.DTO;
using WebApplication1.Services.Interfaces;

namespace WebApplication1.Services;

public class AdminService : IAdminService
{
    private readonly ApplicationDbContext _context;
    private readonly IPointsService _pointsService;
    private readonly IEmailService _emailService;

    public AdminService(ApplicationDbContext context, IPointsService pointsService, IEmailService emailService)
    {
        _context = context;
        _pointsService = pointsService;
        _emailService = emailService;
    }
    
    public async Task<List<AdminUserListDTO>> GetUsersAsync(string? search)
    {
        var query = _context.Users.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.ToLower();
            query = query.Where(u =>
                u.Email.ToLower().Contains(search) ||
                u.FirstName.ToLower().Contains(search) ||
                u.LastName.ToLower().Contains(search));
        }

        var rawUsers = await query
            .OrderByDescending(u => u.UserId)
            .Take(50)
            .Select(u => new
            {
                u.UserId,
                u.Email,
                u.FirstName,
                u.LastName,
                u.Phone,
                u.Role,
                u.IsFlagged,
                u.CreatedAt
            })
            .ToListAsync();

        var userIds = rawUsers.Select(u => u.UserId).ToList();

        var pointBalances = await _context.PointsLots
            .Where(pl => userIds.Contains(pl.UserId)
                         && !pl.IsExpired
                         && (pl.ExpiresAt == null || pl.ExpiresAt > DateTime.UtcNow))
            .GroupBy(pl => pl.UserId)
            .Select(g => new { UserId = g.Key, Balance = g.Sum(pl => pl.PointsRemaining) })
            .ToDictionaryAsync(x => x.UserId, x => x.Balance);

        return rawUsers.Select(u => new AdminUserListDTO
        {
            UserId = u.UserId,
            Email = u.Email,
            FirstName = u.FirstName,
            LastName = u.LastName,
            Phone = u.Phone,
            RoleString = u.Role.ToString(),
            IsFlagged = u.IsFlagged,
            CreatedAt = u.CreatedAt,
            CurrentPoints = pointBalances.ContainsKey(u.UserId) ? pointBalances[u.UserId] : 0
        }).ToList();
    }

    public async Task<bool> ToggleFlagUserAsync(long userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return false;

        user.IsFlagged = !user.IsFlagged;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<AdminUserDetailsDTO?> GetUserDetailsAsync(long userId)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);

        if (user == null) return null;

        var points = await _context.PointsLots
            .Where(pl =>
                pl.UserId == userId && !pl.IsExpired && (pl.ExpiresAt == null || pl.ExpiresAt > DateTime.UtcNow))
            .SumAsync(pl => pl.PointsRemaining);

        var basicInfo = new AdminUserListDTO()
        {
            UserId = user.UserId,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Phone = user.Phone,
            RoleString = user.Role.ToString(),
            IsFlagged = user.IsFlagged,
            CreatedAt = user.CreatedAt,
            CurrentPoints = points
        };
        
        var transactions = await _context.PointsTransactions
            .Where(t => t.SenderId == userId || t.ReceiverId == userId)
            .OrderByDescending(t => t.OccurredAt)
            .Take(10)
            .Select(t => new AdminTransactionSimpleDTO
            {
                Id = t.TrasactionId,
                OccurredAt = t.OccurredAt,
                Type = t.TransactionType.ToString(),
                Amount = t.Points,
                Description = t.SenderId == userId
                    ? $"Sent to User #{t.ReceiverId}"
                    : $"Received from User #{t.SenderId}",
                IsCredit = t.ReceiverId == userId
            })
            .ToListAsync();
        
        var bookings = await _context.BookingRequests
            .Include(br => br.Owner)
            .Include(br => br.Sitter)
            .Where(br => br.OwnerId == userId || br.SitterId == userId)
            .OrderByDescending(br => br.CreatedAt)
            .Take(10)
            .Select(br => new AdminBookingSimpleDTO
            {
                Id = br.BookingId,
                Date = br.StartTime,
                OtherUserName = br.OwnerId == userId
                    ? $"{br.Sitter.FirstName} {br.Sitter.LastName}"
                    : $"{br.Owner.FirstName} {br.Owner.LastName}",
                Status = br.Status.ToString(),
                OfferedPoints = br.OfferedPoints
            }).ToListAsync();

        var reviews = await _context.Reviews
            .Include(r => r.Reviewer)
            .Where(r => r.RevieweeId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .Take(10)
            .Select(r => new AdminReviewSimpleDTO
            {
                Id = r.ReviewId,
                CreatedAt = r.CreatedAt,
                Rating = (r.SitterRating.HasValue
                    ? r.SitterRating.Value
                    : (r.OwnerRating.HasValue ? r.OwnerRating.Value : 0)),
                Content = r.Comment,
                AuthorName = $"{r.Reviewer.FirstName} {r.Reviewer.LastName}"
            }).ToListAsync();

        return new AdminUserDetailsDTO
        {
            UserInfo = basicInfo,
            RecentTransactions = transactions,
            RecentBookings = bookings,
            RecentReviews = reviews
        };
    }

    public async Task<List<AdminReportedReviewDTO>> GetReportedReviewsAsync()
    {
        var reviews = await _context.Reviews
            .Where(r => r.ReviewReports.Any())
            .Include(r => r.Reviewer)
            .Include(r => r.Reviewee)
            .Include(r => r.ReviewReports)
            .ThenInclude(rr => rr.Reporter)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        return reviews.Select(r => new AdminReportedReviewDTO
        {
            ReviewId = r.ReviewId,
            Content = r.Comment,
            Rating = (r.SitterRating ?? r.OwnerRating ?? r.HouseRating ?? r.PetRating ?? 0),
            CreatedAt = r.CreatedAt,
            AuthorName = $"{r.Reviewer.FirstName} {r.Reviewer.LastName}",
            TargetName = $"{r.Reviewee.FirstName} {r.Reviewee.LastName}",
            Reports = r.ReviewReports.Select(rr => new AdminReportDetailDTO
            {
                ReporterName = $"{rr.Reporter.FirstName} {rr.Reporter.LastName}",
                Reason = rr.Reason,
                ReportedAt = rr.ReportedAt
            }).ToList()
        }).ToList();
    }

    public async Task<bool> DeleteReviewAsync(long reviewId)
    {
        var review = await _context.Reviews.FindAsync(reviewId);
        if (review == null) return false;

        _context.Reviews.Remove(review);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DismissReviewReportsAsync(long reviewId)
    {
        var reports = await _context.ReviewReports
            .Where(rr => rr.ReviewId == reviewId)
            .ToListAsync();

        if (!reports.Any()) return false;
        
        _context.ReviewReports.RemoveRange(reports);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<ConversationDTO>> GetUserConversationsAsync(long userId)
    {
        var conversations = await _context.Conversations
            .AsNoTracking()
            .Where(c => c.Participants.Any(p => p.UserId == userId))
            .Include(c => c.Participants)
            .ThenInclude(p => p.User)
            .Include(c => c.Messages)
            .OrderByDescending(c => c.Messages.Max(m => (DateTime?)m.SentAt) ?? c.CreatedAt)
            .ToListAsync();
        
        return conversations.Select(c => new ConversationDTO
        {
            ConversationId = c.ConversationId,
            BookingId = c.BookingId,
            CreatedAt = c.CreatedAt,
            Participants = c.Participants.Select(p => new ConversationParticipantDTO
            {
                ParticipantId = p.ParticipantId,
                UserId = p.UserId,
                UserName = p.User != null ? $"{p.User.FirstName} {p.User.LastName}" : "Deleted User",
                JoinedAt = p.JoinedAt,
                ProfilePhotoUrl = p.User?.ProfilePhotoUrl
            }).ToList(),
            LastMessage = c.Messages.OrderByDescending(m => m.SentAt).Select(m => new MessageDTO
            {
                Content = m.Content,
                SentAt = m.SentAt,
                SenderId = m.SenderId
            }).FirstOrDefault()
        }).ToList();
    }

    public async Task<List<MessageDTO>> GetConversationMessagesAsync(long conversationId)
    {
        var exists = await _context.Conversations.AnyAsync(c => c.ConversationId == conversationId);
        if (!exists) return new List<MessageDTO>();

        return await _context.Messages
            .AsNoTracking()
            .Where(m => m.ConversationId == conversationId)
            .OrderBy(m => m.SentAt)
            .Include(m => m.Sender)
            .Select(m => new MessageDTO
            {
                MessageId = m.MessageId,
                ConversationId = m.ConversationId,
                SenderId = m.SenderId,
                SenderName = m.Sender != null ? $"{m.Sender.FirstName} {m.Sender.LastName}" : "Unknown",
                Content = m.Content,
                SentAt = m.SentAt
            })
            .ToListAsync();
    }

    public async Task<List<PointPolicyDTO>> GetPointPoliciesAsync()
    {
        var policies = _context.PointPolicies.Select(p => new PointPolicyDTO
        {
            PolicyId = p.PolicyId,
            ExpiryDays = p.ExpiryDays,
            MinSpendable = p.MinSpendable,
            PointsPerStay = p.PointsPerHour,
            ServiceType = p.ServiceType.ToString()
        });
        return await policies.ToListAsync();
    }

    public async Task<bool> UpdatePointPolicyAsync(PointPolicyDTO dto)
    {
        var policy = await _context.PointPolicies.FindAsync(dto.PolicyId);
        if (policy == null) return false;
        policy.PointsPerHour = dto.PointsPerStay;
        policy.MinSpendable = dto.MinSpendable;
        policy.ExpiryDays = dto.ExpiryDays;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ProcessRefundAsync(AdminTransactionAdjustmentDTO dto)
    {
        var originalTransaction = await _context.PointsTransactions
            .FirstOrDefaultAsync(t => t.TrasactionId == dto.OriginalTransactionId);

        if (originalTransaction == null) 
            throw new KeyNotFoundException("Original transaction does not exist.");

        if (dto.RefundAmount > originalTransaction.Points)
        {
            throw new InvalidOperationException($"Refund amount {dto.RefundAmount} cannot be more than " +
                                                $"the original transaction amount {originalTransaction.Points}");
        }

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var currentBalance = await _pointsService.GetUserPointsBalanceAsync(originalTransaction.ReceiverId);
            if (currentBalance < dto.RefundAmount)
            {
                throw new InvalidOperationException(
                    $"The receiver does not have enough funds for the refund. Refund failed.");
            }

            await _pointsService.DeductPointsAsync(
                senderId: originalTransaction.ReceiverId,
                receiverId: originalTransaction.SenderId,
                points: dto.RefundAmount,
                type: TransactionType.Adjustment,
                reason: $"REFUND DEBIT: {dto.Reason} (Ref #{originalTransaction.TrasactionId})"
            );

            await _pointsService.CreditPointsAsync(
                receiverId: originalTransaction.SenderId,
                senderId: originalTransaction.ReceiverId,
                points: dto.RefundAmount,
                type: TransactionType.Adjustment,
                reason: $"REFUND CREDIT: {dto.Reason} (Ref #{originalTransaction.TrasactionId})"
            );

            await transaction.CommitAsync();
            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<List<AdminReportedUserDTO>> GetReportedUsersAsync()
    {
        var reports = await _context.UserReports
            .Include(ur => ur.ReportedUser)
            .Include(ur => ur.Reporter)
            .OrderByDescending(ur => ur.CreatedAt)
            .ToListAsync();

        return reports
            .GroupBy(ur => ur.ReportedUserId)
            .Select(g => new AdminReportedUserDTO
            {
                UserId = g.Key,
                FullName = $"{g.First().ReportedUser.FirstName} {g.First().ReportedUser.LastName}",
                Email = g.First().ReportedUser.Email,
                IsFlagged = g.First().ReportedUser.IsFlagged,
                IsBanned = g.First().ReportedUser.IsBanned,
                ReportCount = g.Count(),
                LastReportDate = g.Max(r => r.CreatedAt),
                Reports = g.Select(r => new AdminReportDetailDTO
                {
                    ReportId = r.ReportId,
                    ReporterName = $"{r.Reporter.FirstName} {r.Reporter.LastName}",
                    Reason = r.Reason,
                    ReportedAt = r.CreatedAt
                }).ToList()
            }).OrderByDescending(u => u.ReportCount).ToList();
    }

    public async Task<bool> BanUserAsync(long userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return false;

        user.IsBanned = true;

        await _context.SaveChangesAsync();

        await _emailService.SendAccountBannedEmail(user.Email, user.FirstName,
            "Violations of our community standards.");

        return true;
    }

    public async Task<bool> DismissUserReportsAsync(long userId)
    {
        var reports = await _context.UserReports.Where(ur => ur.ReportedUserId == userId).ToListAsync();
        _context.UserReports.RemoveRange(reports);

        var user = await _context.Users.FindAsync(userId);
        if (user != null)
        {
            user.IsFlagged = false;
            
        }
        
        await _context.SaveChangesAsync();
        return true;
    }
    
    public async Task<bool> DismissSingleUserReportAsync(long reportId)
    {
        var report = await _context.UserReports.FindAsync(reportId);
        if (report == null) return false;

        _context.UserReports.Remove(report);
        await _context.SaveChangesAsync();
        return true;
    }
}