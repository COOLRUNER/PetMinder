using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetMinder.Models;

public class EmailVerificationToken
{
    [Key]
    public Guid Token { get; set; } = Guid.NewGuid();

    [Required, ForeignKey(nameof(User))]
    public long UserId { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddHours(24);

    public bool IsUsed { get; set; } = false;

    public virtual User User { get; set; }
}