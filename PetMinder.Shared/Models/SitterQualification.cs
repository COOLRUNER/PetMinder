using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetMinder.Models
{
    public class SitterQualification
    {
        [Key]
        public long SitterQualificationId { get; set; }

        [Required]
        [ForeignKey(nameof(Sitter))]
        public long SitterId { get; set; }

        [Required]
        [ForeignKey(nameof(QualificationType))]
        public long QualificationTypeId { get; set; }

        [Required]
        public DateTime GrantedAt { get; set; } = DateTime.UtcNow;

        public virtual User Sitter { get; set; }

        public virtual QualificationType QualificationType { get; set; }

    }
}