using System.ComponentModel.DataAnnotations;
using PetMinder.Models;

namespace PetMinder.Shared.DTO;

public class UpdateUserDTO
{
    [StringLength(100)]
    public string? FirstName { get; set; }

    [StringLength(100)]
    public string? LastName { get; set; }

    [Phone, StringLength(9)]
    public string? Phone { get; set; }

    [StringLength(512)]
    public string? ProfilePhotoUrl { get; set; }

    public UserRole? AddRoles { get; set; }

    public int? MinPoints { get; set; }
    
    public UserRole? RemoveRoles { get; set; }
}