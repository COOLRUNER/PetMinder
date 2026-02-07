using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetMinder.Models
{
    public class PointsTransaction
    {
        [Key]
        public long TrasactionId { get; set; }

        [Required]
        [ForeignKey(nameof(Sender))]
        public long SenderId { get; set; }

        [Required]
        [ForeignKey(nameof(Receiver))]
        public long ReceiverId { get; set; }

        [Required]
        [ForeignKey(nameof(PointPolicy))]
        public long PolicyId { get; set; }

        [Required]
        public int Points { get; set; }

        [Required]
        public TransactionType TransactionType { get; set; }

        [Required]
        public DateTime OccurredAt { get; set; } = DateTime.UtcNow;

        public string Reason { get; set; }

        public virtual User Sender { get; set; }

        public virtual User Receiver { get; set; }

        public virtual PointPolicy PointPolicy { get; set; }

        public virtual PointDonation PointDonation { get; set; }

    }
}
