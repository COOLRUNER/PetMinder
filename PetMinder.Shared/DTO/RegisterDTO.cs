using PetMinder.Models;
using System.ComponentModel.DataAnnotations;

namespace PetMinder.Shared.DTO
{
    public class RegisterDTO
    {
        [Required(ErrorMessage = "Email is required"), EmailAddress(ErrorMessage = "Invalid email format"), StringLength(255)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required"), StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please confirm your password"), Compare(nameof(Password), ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "First name is required"), StringLength(50, ErrorMessage = "First name is too long")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required"), StringLength(50, ErrorMessage = "Last name is too long")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Phone number is required"), StringLength(9, MinimumLength = 9, ErrorMessage = "Phone must be exactly 9 digits"), RegularExpression(@"^\d{9}$", ErrorMessage = "Phone must contain only digits")]
        public string Phone { get; set; }

        public UserRole DesiredRoles { get; set; } = UserRole.None; 

        public string? Fingerprint { get; set; }
        public string? ReferralCode { get; set; }
    }
}

