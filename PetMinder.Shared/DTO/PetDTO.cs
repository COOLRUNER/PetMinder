using System.ComponentModel.DataAnnotations;
using PetMinder.Models;

namespace PetMinder.Shared.DTO
{

    public class CreatePetDTO
    {
        [Required, StringLength(100)]
        public string Name { get; set; }
        [Required, StringLength(100)]
        public string Breed { get; set; }
        [Required, Range(0, 100, ErrorMessage = "Age must be between 0 and 100.")]
        public int Age { get; set; }

        [Required]
        public PetType Type { get; set; }

        [Required]
        public PetBehaviorComplexity BehaviorComplexity { get; set; }

        public string? HealthNotes { get; set; }
        public string? BehaviorNotes { get; set; }
        [StringLength(512)]
        public string? PhotoUrl { get; set; }
    }

    public class UpdatePetDTO
    {
        [Required]
        public long PetId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Breed { get; set; }
        [Required, Range(0, 100, ErrorMessage = "Age must be between 0 and 100.")]
        public int Age { get; set; }

        [Required]
        public PetType Type { get; set; }

        [Required]
        public PetBehaviorComplexity BehaviorComplexity { get; set; }

        public string? HealthNotes { get; set; }
        public string? BehaviorNotes { get; set; }
        [StringLength(512)]
        public string? PhotoUrl { get; set; }
    }

    public class PetDTO
    {
        public long PetId { get; set; }
        public string Name { get; set; }
        public string Breed { get; set; }
        public int Age { get; set; }
        public PetType Type { get; set; }
        public PetBehaviorComplexity BehaviorComplexity { get; set; }
        public string? HealthNotes { get; set; }
        public string? BehaviorNotes { get; set; }
        public string? PhotoUrl { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}