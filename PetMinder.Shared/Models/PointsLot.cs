using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetMinder.Models;

public class PointsLot
{
    [Key]
    public long LotId { get; set; }
    
    [Required]
    public long UserId { get; set; }
    
    [Required]
    [ForeignKey(nameof(PointsTransaction))]
    public long TransactionId { get; set; }
    
    [Required]
    public int PointsIssued { get; set; }
    
    [Required]
    public int PointsRemaining { get; set; }
    
    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? ExpiresAt { get; set; }
    
    public bool IsExpired { get; set; }
    
    public virtual PointsTransaction PointsTransaction { get; set; }
}