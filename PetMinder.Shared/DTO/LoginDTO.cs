using System.ComponentModel.DataAnnotations;

namespace PetMinder.Shared.DTO
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "Email is required"), EmailAddress(ErrorMessage = "Invalid email format"), StringLength(255)]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is required"), StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; }
    }
}

