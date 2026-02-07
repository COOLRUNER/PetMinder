using PetMinder.Shared.DTO;

namespace WebApplication1.Services.Interfaces;

public interface IReportService
{
    Task ReportUserAsync(long userId, ReportUserDTO reportUserDto);
    Task ReportReviewAsync(long reporterId, long reviewId, string reason);
}