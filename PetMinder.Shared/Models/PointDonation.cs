using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetMinder.Models
{
    public class PointDonation
    {
        [Key]
        public long DonationId { get; set; }
        
        [Required]
        [ForeignKey(nameof(Shelter))]
        public long ShelterId { get; set; }

        [Required]
        [ForeignKey(nameof(PointsTransaction))]
        public long PointsTransactionId { get; set; }

        public virtual Shelter Shelter { get; set; }

        public virtual PointsTransaction PointsTransaction { get; set; }
    }
}
