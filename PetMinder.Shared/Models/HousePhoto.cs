using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetMinder.Models
{
    public class HousePhoto
    {
        [Key]
        public long PhotoId { get; set; }

        [Required]
        [ForeignKey(nameof(Listing))]
        public long ListingId { get; set; }

        [Required, StringLength(255)]
        public string PhotoUrl { get; set; }

        [Required]
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        public virtual HouseListing Listing { get; set; }
    }
}
