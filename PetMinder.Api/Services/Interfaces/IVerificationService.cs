using PetMinder.Models;

namespace WebApplication1.Services.Interfaces;

public interface IVerificationService
{
    Task CompleteVerificationStep(long userId, VerificationStep step);
    Task<bool> IsVerificationStepComplete(long userId, VerificationStep step);
    Task<List<VerificationStep>> GetCompletedSteps(long userId);
    Task<bool> IsFullyVerified(long userId);
}