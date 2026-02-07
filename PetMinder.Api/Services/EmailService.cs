using FluentEmail.Core;
using Microsoft.EntityFrameworkCore;
using PetMinder.Data;
using PetMinder.Models;
using WebApplication1.Services.Interfaces;

namespace WebApplication1.Services;

public class EmailService : IEmailService
{
    private readonly ApplicationDbContext _context;
    private readonly IFluentEmail _fluentEmail;
    private readonly IVerificationService _verificationService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(ApplicationDbContext context, IFluentEmail fluentEmail,
        IVerificationService verificationService, IConfiguration configuration, ILogger<EmailService> logger)
    {
        _context = context;
        _fluentEmail = fluentEmail;
        _verificationService = verificationService;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendVerificationEmail(long userId, string emailAddress)
    {
        try
        {
            var user = await _context.Users
                .Include(u => u.EmailVerificationTokens)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
            {
                throw new InvalidOperationException("User does not exist.");
            }

            _context.EmailVerificationTokens.RemoveRange(user.EmailVerificationTokens);

            var newToken = new EmailVerificationToken { UserId = userId };
            _context.EmailVerificationTokens.Add(newToken);
            await _context.SaveChangesAsync();

            var frontUrl = _configuration["Frontend:BaseUrl"]?.Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries)
                .FirstOrDefault()?.Trim().TrimEnd('/');
            var verificationLink = $"{frontUrl}/verify-email?token={newToken.Token}";

            _logger.LogInformation("Verification link for email: {Email} and link: {Link}", emailAddress,
                verificationLink);

            var email = _fluentEmail
                .To(emailAddress)
                .Subject("PetMinder: Verify your account")
                .Body($"""
                       <h1>Welcome to PetMinder!</h1>
                       <p>Please click the link below to verify your email address and receive your 50 bonus points!</p>
                       <p><a href="{verificationLink}">{verificationLink}</a></p>
                       <p>This link will expire in 24 hours.</p>
                       <p>If you didn't create this account, please ignore this email.</p>
                       """, true);

            _logger.LogInformation("Attempting to send verification email to {Email} via SMTP...", emailAddress);
            
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(15));
            var result = await email.SendAsync(cts.Token);
            
            if (result.Successful)
            {
                _logger.LogInformation("Verification email sent to {Email}", emailAddress);
            }
            else
            {
                _logger.LogError("Failed to send verification email to {Email}. Errors: {Errors}", 
                    emailAddress, string.Join(", ", result.ErrorMessages));
                throw new InvalidOperationException($"Failed to send email: {string.Join(", ", result.ErrorMessages)}");
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogError("Email sending timed out for {Email} after 15 seconds.", emailAddress);
            throw new InvalidOperationException("Email sending timed out. This is likely a network restriction on the server (firewall blocking SMTP port 587).");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to send verification email to {Email}", emailAddress);
            throw;
        }
    }

    public async Task<bool> VerifyEmailToken(Guid token)
    {
        var existingToken = await _context.EmailVerificationTokens
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.Token == token);

        if (existingToken == null || existingToken.IsUsed || existingToken.ExpiresAt < DateTime.UtcNow)
        {
            return false;
        }

        existingToken.IsUsed = true;
        await _context.SaveChangesAsync();

        await _verificationService.CompleteVerificationStep(existingToken.UserId, VerificationStep.EmailVerification);

        return true;
    }

    public async Task SendAccountBannedEmail(string email, string name, string reason)
    {
        try
        {
            var subject = "Important: Your PetMinder Account has been suspended";
            var body = $"""
                        <h1>Account Suspended</h1>
                        <p>Hello {name},</p>
                        <p>Your account has been permanently suspended due to a violation of our Community Guidelines.</p>
                        <p><strong>Reason:</strong> {reason}</p>
                        <p>If you believe this is an error, please reply back to this email.</p>
                        """;
            
            var result = await _fluentEmail
                .To(email)
                .Subject(subject)
                .Body(body, true)
                .SendAsync();
            
            if (result.Successful)
            {
                _logger.LogInformation("Ban email sent to {Email}", email);
            }
            else
            {
                _logger.LogError("Failed to send ban email to {Email}. Errors: {Errors}", 
                    email, string.Join(", ", result.ErrorMessages));
            }
        } 
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send ban email to {Email}", email);
        }
    }
}