using System.ComponentModel.DataAnnotations;
using PetMinder.Models;

namespace PetMinder.Shared.DTO
{
    public class CreateAddressDTO
    {
        [Required, StringLength(255)]
        public string Street { get; set; }

        [Required, StringLength(100)]
        public string City { get; set; }

        [Required, StringLength(6, MinimumLength = 5)]
        public string ZipCode { get; set; }
    
        [Required]
        public AddressType Type { get; set; } = AddressType.Primary;
    }
    
    public class AddressDTO
    {
        public long UserAddressId { get; set; }
        public long AddressId { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string ZipCode { get; set; }
        public AddressType Type { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
