using PetMinder.Models;
using PetMinder.Shared.DTO;

namespace WebApplication1.Services.Interfaces
{
    public interface IPetService
    {
        Task<PetDTO> AddPetAsync(long userId, CreatePetDTO dto);
        Task<PetDTO> UpdatePetAsync(long userId, UpdatePetDTO dto);
        Task<bool> DeletePetAsync(long userId, long petId);
        Task<List<PetDTO>> ListPetsAsync(long userId);
    }
}
