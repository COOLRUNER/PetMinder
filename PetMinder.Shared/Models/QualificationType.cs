using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace PetMinder.Models
{
    [Index(nameof(Code), IsUnique = true)]
    public class QualificationType
    {
        [Key]
        public long QualificationTypeId { get; set; }

        [Required, StringLength(50)]
        public string Code { get; set; }

        public string Description { get; set; }

        public virtual ICollection<SitterQualification> SitterQualifications { get; set; } = new List<SitterQualification>();

    }
}
