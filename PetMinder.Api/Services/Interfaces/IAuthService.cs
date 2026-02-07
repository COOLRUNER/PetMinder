using PetMinder.Models;
using PetMinder.Shared.DTO;

namespace WebApplication1.Services.Interfaces
{
    public interface IAuthService
    {
        Task<User> RegisterAsync(RegisterDTO registerDTO);
        Task<AuthResultDTO> LoginAsync(LoginDTO loginDTO);
        Task<bool> UpdateUserProfileAsync(long userId, UpdateUserDTO dto);
        Task<AuthResultDTO> RefreshTokenAsync(long userId);
    }
}
