using System.ComponentModel.DataAnnotations;

namespace PetMinder.Shared.DTO;

public class QualificationTypeDTO
{
    public long QualificationTypeId { get; set; }
    public string Code { get; set; } 
    public string Name { get; set; } 
    public string Description { get; set; }
}

public class AddSitterQualificationDTO
{
    [Required]
    public long QualificationTypeId { get; set; }
}

public class SitterQualificationDTO
{
    public long SitterQualificationId { get; set; }
    public long SitterId { get; set; }
    public QualificationTypeDTO QualificationType { get; set; } 
    public string Details { get; set; } 
    public DateTime GrantedAt { get; set; } 
}