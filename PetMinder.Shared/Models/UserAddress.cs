using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetMinder.Models;

public class UserAddress
{
   [Key]
   public long UserAddressId { get; set; }
   
   [Required]
   [ForeignKey(nameof(User))]
   public long UserId { get; set; }
   public virtual User User { get; set; }

   [Required]
   [ForeignKey(nameof(Address))]
   public long AddressId { get; set; }
   public virtual Address Address { get; set; }
        
   [Required]
   public AddressType Type { get; set; } = AddressType.Primary;
}