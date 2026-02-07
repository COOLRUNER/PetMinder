using System.ComponentModel.DataAnnotations;

namespace PetMinder.Shared.DTO
{
    public class TransactionDTO
    {
        public long TransactionId { get; set; }
        public long SenderId { get; set; }
        public string? SenderName { get; set; }
        public long ReceiverId { get; set; }
        public string? ReceiverName { get; set; }
        public int Points { get; set; }
        public string TransactionType { get; set; }
        public DateTime OccurredAt { get; set; }
        public string Reason { get; set; }
        public long PolicyId { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }

    public class AdminTransactionAdjustmentDTO
    {
        [Required]
        public long OriginalTransactionId { get; set; }
        
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Refund amount must be positive")]
        public int RefundAmount { get; set; } 
        
        [Required]
        [MinLength(5, ErrorMessage = "Please provide a reason")]
        public string Reason { get; set; }
    }
}
