using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetMinder.Models;

public class UserVerificationStep
{
    [Key]
    public long UserVerificationStepId { get; set; }
    
    [Required, ForeignKey(nameof(User))]
    public long UserId { get; set; }
    
    [Required]
    public VerificationStep Step { get; set; }
    
    [Required]
    public DateTime CompletedAt { get; set; } = DateTime.UtcNow;
    
    [Required]
    public int PointsAwarded { get; set; }
    
    public virtual User User { get; set; }
}