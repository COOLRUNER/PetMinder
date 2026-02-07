namespace WebApplication1.Services.Interfaces;

public interface IEmailService
{
    Task SendVerificationEmail(long userId, string emailAddress);
    Task<bool> VerifyEmailToken(Guid token);
    Task SendAccountBannedEmail(string email, string name, string reason);
}