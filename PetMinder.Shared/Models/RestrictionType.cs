using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace PetMinder.Models
{
    [Index(nameof(Code), IsUnique = true)]
    public class RestrictionType
    {
        [Key]
        public long RestrictionTypeId { get; set; }

        [Required, StringLength(50)]
        public string Code { get; set; }

        public string Description { get; set; }

        public virtual ICollection<SitterRestriction> SitterRestrictions { get; set; } = new List<SitterRestriction>();

    }
}
