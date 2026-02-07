using System;

namespace PetMinder.Shared.DTO;

public class PointsLotDTO
{
    public long LotId { get; set; }
    public int PointsIssued { get; set; }
    public int PointsRemaining { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public bool IsExpired { get; set; }
    public string TransactionReason { get; set; } = string.Empty;
}
