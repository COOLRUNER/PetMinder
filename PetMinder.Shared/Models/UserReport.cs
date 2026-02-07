using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetMinder.Models;

public class UserReport
{
    [Key]
    public long ReportId { get; set; }

    [Required]
    [ForeignKey(nameof(Reporter))]
    public long ReporterId { get; set; } 

    [Required]
    [ForeignKey(nameof(ReportedUser))]
    public long ReportedUserId { get; set; }

    [Required, StringLength(500)]
    public string Reason { get; set; }
    
    [StringLength(50)]
    public string Source { get; set; } 

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public virtual User Reporter { get; set; }
    public virtual User ReportedUser { get; set; }
}