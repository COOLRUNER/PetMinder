using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PetMinder.Data;
using PetMinder.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using PetMinder.Api.Services;
using PetMinder.Shared.DTO;
using WebApplication1.Services.Interfaces;

namespace WebApplication1.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher<User> _passwordHasher;
        private readonly IConfiguration _config;
        private readonly IEmailService _emailService;
        private readonly IReferralService _referralService;
        private readonly ILogger<AuthService> _logger;
        private readonly IVerificationService _verificationService;

        public AuthService(ApplicationDbContext context, IConfiguration config, IEmailService emailService,
            ILogger<AuthService> logger, IReferralService referralService, IVerificationService verificationService)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<User>();
            _config = config;
            _emailService = emailService;
            _referralService = referralService;
            _logger = logger;
            _verificationService = verificationService;
        }

        public async Task<User> RegisterAsync(RegisterDTO registerDTO)
        {
            // if saving a user succeeds but fingerprint fails, then rollback the both changes.
            var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                if (await _context.Users.AnyAsync(u => u.Email == registerDTO.Email))
                {
                    throw new InvalidOperationException("Email already in use.");
                }

                if (await _context.Users.AnyAsync(u => u.Phone == registerDTO.Phone))
                {
                    throw new InvalidOperationException("Phone number already in use.");
                }

                if (!string.IsNullOrEmpty(registerDTO.Fingerprint))
                {
                    if (await _context.DeviceFingerprints.AnyAsync(df => df.Fingerprint == registerDTO.Fingerprint))
                    {
                        throw new InvalidOperationException("This device is already associated with another account. Please use your existing account.");
                    }
                }

                UserRole initialRoles = UserRole.BasicUser;
                if (registerDTO.DesiredRoles.HasFlag(UserRole.Owner))
                {
                    initialRoles |= UserRole.Owner;
                }
                if (registerDTO.DesiredRoles.HasFlag(UserRole.Sitter))
                {
                    initialRoles |= UserRole.Sitter;
                }

                var user = new User
                {
                    Email = registerDTO.Email,
                    FirstName = registerDTO.FirstName,
                    LastName = registerDTO.LastName,
                    Phone = registerDTO.Phone,
                    Role = initialRoles,
                    CreatedAt = DateTime.UtcNow
                };

                user.PasswordHash = _passwordHasher.HashPassword(user, registerDTO.Password);

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                if (!string.IsNullOrEmpty(registerDTO.Fingerprint))
                {
                    var fingerprint = new DeviceFingerprint
                    {
                        UserId = user.UserId,
                        Fingerprint = registerDTO.Fingerprint,
                        CreatedAt = DateTime.UtcNow
                    };
                    _context.DeviceFingerprints.Add(fingerprint);
                }

                if (initialRoles.HasFlag(UserRole.Sitter))
                {
                    _context.SitterSettings.Add(new SitterSettings
                    {
                        SitterId = user.UserId,
                        MinPoints = 0
                    });
                }

                await _context.SaveChangesAsync();

                if (!string.IsNullOrEmpty(registerDTO.ReferralCode))
                {
                    await _referralService.ApplyReferralAsync(registerDTO.ReferralCode, user.UserId);
                }

                await _referralService.GetOrCreateReferralCodeAsync(user.UserId);

                await transaction.CommitAsync();

                return user;
            }
            catch (InvalidOperationException ex)
            {
                await transaction.RollbackAsync();
                _logger.LogWarning("Registration failed: {Message}", ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Registration failed for email {Email}", registerDTO.Email);
                throw new InvalidOperationException("An unexpected error occurred during registration. Please try again later.", ex);
            }
        }

        public async Task<AuthResultDTO> LoginAsync(LoginDTO loginDTO)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginDTO.Email);
            if (user == null)
            {
                throw new InvalidOperationException("User not found.");
            }
            
            if (user != null && user.IsBanned)
            {
                throw new UnauthorizedAccessException("Your account has been suspended. Please contact support.");
            }

            var res = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginDTO.Password);
            if (res == PasswordVerificationResult.Success || res == PasswordVerificationResult.SuccessRehashNeeded)
            {
                if (res == PasswordVerificationResult.SuccessRehashNeeded)
                {
                    user.PasswordHash = _passwordHasher.HashPassword(user, loginDTO.Password);
                    await _context.SaveChangesAsync();
                }

                var token = GenerateJwtToken(user);
                return new AuthResultDTO
                {
                    Token = token,
                    UserId = user.UserId,
                    Email = user.Email,
                    Role = user.Role.ToString()
                };
            }
            else
            {
                throw new InvalidOperationException("Invalid password.");
            }
        }

        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _config.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            foreach (UserRole roleFlag in Enum.GetValues(typeof(UserRole)))
            {
                if (roleFlag != UserRole.None && user.Role.HasFlag(roleFlag))
                {
                    claims.Add(new Claim(ClaimTypes.Role, roleFlag.ToString()));
                }
            }

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["ExpiresInMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<bool> UpdateUserProfileAsync(long userId, UpdateUserDTO dto)
        {
            var user = await _context.Users.Include(u => u.SitterSettings).FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
            {
                return false;
            }

            if (!string.IsNullOrEmpty(dto.FirstName))
            {
                user.FirstName = dto.FirstName;
            }
            if (!string.IsNullOrEmpty(dto.LastName))
            {
                user.LastName = dto.LastName;
            }

            if (!string.IsNullOrEmpty(dto.Phone) && user.Phone != dto.Phone)
            {
                if (dto.Phone.Length != 9 || !dto.Phone.All(char.IsDigit))
                {
                    throw new InvalidOperationException("Phone must be 9 digits");
                }

                var existingUserWithPhone = await _context.Users.FirstOrDefaultAsync(u => u.Phone == dto.Phone && u.UserId != userId);
                if (existingUserWithPhone != null)
                {
                    throw new InvalidOperationException("The provided phone number is already in use by another account.");
                }
                user.Phone = dto.Phone;
            }

            if (dto.ProfilePhotoUrl != null)
            {
                if (!Uri.TryCreate(dto.ProfilePhotoUrl, UriKind.RelativeOrAbsolute, out _))
                {
                    throw new InvalidOperationException("Invalid profile photo URL");
                }

                user.ProfilePhotoUrl = dto.ProfilePhotoUrl;
                await _verificationService.CompleteVerificationStep(userId, VerificationStep.ProfilePhotoUpload);
            }

            if (dto.AddRoles.HasValue && dto.AddRoles.Value != UserRole.None)
            {
                UserRole rolesToAdd = dto.AddRoles.Value;

                if (rolesToAdd.HasFlag(UserRole.Admin))
                {
                    rolesToAdd &= ~UserRole.Admin;
                }

                if (dto.AddRoles.HasValue && dto.AddRoles.Value.HasFlag(UserRole.Sitter))
                {
                    if (!user.Role.HasFlag(UserRole.Sitter))
                    {
                        if (user.SitterSettings == null)
                        {
                            user.SitterSettings = new SitterSettings
                            {
                                MinPoints = 0
                            };
                        }
                    }
                }

                user.Role |= rolesToAdd;
            }
            
            if (dto.RemoveRoles.HasValue && dto.RemoveRoles.Value != UserRole.None)
            {
                UserRole rolesToRemove = dto.RemoveRoles.Value;

                if (rolesToRemove.HasFlag(UserRole.BasicUser))
                    rolesToRemove &= ~UserRole.BasicUser;

                if (rolesToRemove.HasFlag(UserRole.Admin))
                    rolesToRemove &= ~UserRole.Admin;

                user.Role &= ~rolesToRemove; 
            }

            if (dto.MinPoints.HasValue)
            {
                if (user.Role.HasFlag(UserRole.Sitter))
                {
                    if (user.SitterSettings == null)
                    {
                        user.SitterSettings = new SitterSettings();
                    }
                    user.SitterSettings.MinPoints = dto.MinPoints.Value;
                }
                else
                {
                    throw new InvalidOperationException("Only sitters can set a minimum points requirement.");
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<AuthResultDTO> RefreshTokenAsync(long userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
            {
                throw new InvalidOperationException("User not found.");
            }

            var token = GenerateJwtToken(user);
            return new AuthResultDTO
            {
                Token = token,
                UserId = user.UserId,
                Email = user.Email,
                Role = user.Role.ToString()
            };
        }
    }
}
