using System.ComponentModel.DataAnnotations;

namespace PetMinder.Models;

public class Address
{
    [Key]
    public long AddressId { get; set; }
    
    [Required, StringLength(255)]
    public string Street { get; set; }
    
    [Required, StringLength(255)]
    public string City { get; set; }
    
    [Required, StringLength(100)]
    public string Country { get; set; } = "Poland";
    
    [Required, StringLength(6)]
    public string ZipCode { get; set; }
    
    [Required]
    public double Latitude { get; set; }
    
    [Required]
    public double Longitude { get; set; }
    
    public virtual ICollection<UserAddress> UserAddresses { get; set; } = new List<UserAddress>();
}