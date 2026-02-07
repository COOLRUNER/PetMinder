using PetMinder.Shared.DTO;

namespace WebApplication1.Services.Interfaces;

public interface IAdminService
{
    Task<List<AdminUserListDTO>> GetUsersAsync(string? search);
    Task<bool> ToggleFlagUserAsync(long userId);
    Task<AdminUserDetailsDTO?> GetUserDetailsAsync(long userId);
    Task<List<AdminReportedReviewDTO>> GetReportedReviewsAsync();
    Task<bool> DeleteReviewAsync(long reviewId);
    Task<bool> DismissReviewReportsAsync(long reviewId);
    Task<List<ConversationDTO>> GetUserConversationsAsync(long userId);
    Task<List<MessageDTO>> GetConversationMessagesAsync(long conversationId);
    Task<List<PointPolicyDTO>> GetPointPoliciesAsync();
    Task<bool> UpdatePointPolicyAsync(PointPolicyDTO dto);
    Task<bool> ProcessRefundAsync(AdminTransactionAdjustmentDTO dto);
    Task<List<AdminReportedUserDTO>> GetReportedUsersAsync();
    Task<bool> BanUserAsync(long userId);
    Task<bool> DismissUserReportsAsync(long userId);
    Task<bool> DismissSingleUserReportAsync(long reportId);
}