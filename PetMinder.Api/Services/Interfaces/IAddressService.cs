using PetMinder.Shared.DTO;

namespace WebApplication1.Services.Interfaces;

public interface IAddressService
{
    Task<AddressDTO> AddAddressAsync(long userId, CreateAddressDTO dto);
    Task<List<AddressDTO>> GetUserAddressesAsync(long userId);
    Task<bool> DeleteUserAddressAsync(long userId, long userAddressId);
}