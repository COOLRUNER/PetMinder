using System.ComponentModel.DataAnnotations;

namespace PetMinder.Shared.DTO;

public class RestrictionTypeDTO
{
    public long RestrictionTypeId { get; set; }
    public string Code { get; set; } 
    public string Name { get; set; } 
    public string Description { get; set; }
}

public class AddSitterRestrictionDTO
{
    [Required]
    public long RestrictionTypeId { get; set; }
}

public class SitterRestrictionDTO
{
    public long SitterRestrictionId { get; set; }
    public long SitterId { get; set; }
    public RestrictionTypeDTO RestrictionType { get; set; } 
    public string Details { get; set; } 
    public DateTime SetAt { get; set; } 
}