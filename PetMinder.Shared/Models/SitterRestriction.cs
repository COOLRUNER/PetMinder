using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetMinder.Models
{
    public class SitterRestriction
    {
        [Key]
        public long SitterRestrictionId { get; set; }

        [Required]
        [ForeignKey(nameof(Sitter))]
        public long SitterId { get; set; }

        [Required]
        [ForeignKey(nameof(RestrictionType))]
        public long RestrictionTypeId { get; set; }

        [Required]
        public DateTime SetAt { get; set; } = DateTime.UtcNow;

        public virtual User Sitter { get; set; }

        public virtual RestrictionType RestrictionType { get; set; }

    }
}