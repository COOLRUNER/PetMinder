using Microsoft.EntityFrameworkCore;
using PetMinder.Data;
using PetMinder.Models;
using PetMinder.Shared.DTO;
using WebApplication1.Services.Interfaces;

namespace WebApplication1.Services;

public class ReportService : IReportService
{
    private readonly ApplicationDbContext _context;

    public ReportService(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task ReportUserAsync(long userId, ReportUserDTO reportUserDto)
    {
        if (userId == reportUserDto.ReportedUserId)
        {
            throw new InvalidOperationException("You cannot report yourself.");
        }

        var reportedUserExists = await _context.Users.AnyAsync(u => u.UserId == reportUserDto.ReportedUserId);
        if (!reportedUserExists)
        {
            throw new KeyNotFoundException("User you want to report does not exist.");
        }

        var alreadyReported = await _context.UserReports
            .AnyAsync(ur => ur.ReporterId == userId && ur.ReportedUserId == reportUserDto.ReportedUserId 
                                                    && ur.Source == reportUserDto.Source);

        if (alreadyReported)
        {
            throw new InvalidOperationException("You already reported this user. Your report is being reviewed.");
        }

        var report = new UserReport
        {
            ReporterId = userId,
            CreatedAt = DateTime.UtcNow,
            Reason = reportUserDto.Reason,
            ReportedUserId = reportUserDto.ReportedUserId,
            Source = reportUserDto.Source
        };

        _context.UserReports.Add(report);

        int existingReportsCount =
            await _context.UserReports.CountAsync(ur => ur.ReportedUserId == reportUserDto.ReportedUserId);

        if (existingReportsCount + 1 >= 5) 
        {
            var userToFlag = await _context.Users.FindAsync(reportUserDto.ReportedUserId);
            if (userToFlag != null && !userToFlag.IsFlagged)
            {
                userToFlag.IsFlagged = true;
            }
        }
        await _context.SaveChangesAsync();
    }

    public async Task ReportReviewAsync(long reporterId, long reviewId, string reason)
    {
        if (await _context.Reviews.AnyAsync(r => r.ReviewId == reviewId && r.ReviewerId == reporterId))
        {
            throw new InvalidOperationException("You cannot report your own review.");
        }

        if (await _context.ReviewReports.AnyAsync(rr => rr.ReviewId == reviewId && rr.ReporterId == reporterId))
        {
            throw new InvalidOperationException("You have already reported this review.");
        }

        var report = new ReviewReport
        {
            ReviewId = reviewId,
            ReporterId = reporterId,
            Reason = reason,
            ReportedAt = DateTime.UtcNow
        };

        _context.ReviewReports.Add(report);
        await _context.SaveChangesAsync();
    }
}