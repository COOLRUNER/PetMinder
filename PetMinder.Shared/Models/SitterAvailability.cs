using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetMinder.Models
{
    public class SitterAvailability
    {
        [Key]
        public long AvailabilityId { get; set; }

        [Required]
        [ForeignKey(nameof(Sitter))]
        public long SitterId { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }
        public virtual User Sitter { get; set; }
    }
}
