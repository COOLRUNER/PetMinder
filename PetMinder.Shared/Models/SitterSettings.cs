using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetMinder.Models
{
    public class SitterSettings
    {
        [Key, ForeignKey(nameof(User))]
        public long SitterId { get; set; }
        
        [Required]
        public int MinPoints { get; set; }

        public virtual User User { get; set; }
    }
}
