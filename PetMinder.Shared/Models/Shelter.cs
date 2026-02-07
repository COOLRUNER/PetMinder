using System.ComponentModel.DataAnnotations;

namespace PetMinder.Models
{
    public class Shelter
    {
        [Key]
        public long ShelterId { get; set; }

        [Required, StringLength(255)]
        public string Name { get; set; }

        public string ContactInfo { get; set; }

        public virtual ICollection<PointDonation> PointDonations { get; set; } = new List<PointDonation>();

    }
}
