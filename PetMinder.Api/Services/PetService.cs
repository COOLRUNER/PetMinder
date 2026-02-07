using Microsoft.EntityFrameworkCore;
using PetMinder.Data;
using PetMinder.Models;
using PetMinder.Shared.DTO;
using WebApplication1.Services.Interfaces;

namespace WebApplication1.Services
{
    public class PetService : IPetService
    {
        private readonly ApplicationDbContext _context;
        public PetService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PetDTO> AddPetAsync(long userId, CreatePetDTO dto)
        {
            bool hasExistingPets = await _context.Pets.AnyAsync(p => p.UserId == userId);

            var pet = new Pet
            {
                UserId = userId,
                Name = dto.Name,
                Breed = dto.Breed,
                Age = dto.Age,
                Type = dto.Type,
                BehaviorComplexity = dto.BehaviorComplexity,
                HealthNotes = dto.HealthNotes,
                BehaviorNotes = dto.BehaviorNotes,
                PhotoUrl = dto.PhotoUrl,
                CreatedAt = DateTime.UtcNow
            };
            _context.Pets.Add(pet);

            if (!hasExistingPets)
            {
                var user = await _context.Users.FindAsync(userId);
                if (user != null && !user.Role.HasFlag(UserRole.Owner))
                {
                    user.Role |= UserRole.Owner;
                }
            }

            await _context.SaveChangesAsync();
            return ToPetDTO(pet);
        }

        public async Task<PetDTO> UpdatePetAsync(long userId, UpdatePetDTO dto)
        {
            var pet = await _context.Pets.FirstOrDefaultAsync(p => p.PetId == dto.PetId && p.UserId == userId);
            if (pet == null) throw new KeyNotFoundException("Pet not found or not owned by user.");
            pet.Name = dto.Name;
            pet.Breed = dto.Breed;
            pet.Age = dto.Age;
            pet.Type = dto.Type;
            pet.BehaviorComplexity = dto.BehaviorComplexity;
            pet.HealthNotes = dto.HealthNotes;
            pet.BehaviorNotes = dto.BehaviorNotes;
            pet.PhotoUrl = dto.PhotoUrl;
            await _context.SaveChangesAsync();
            return ToPetDTO(pet);
        }

        public async Task<bool> DeletePetAsync(long userId, long petId)
        {
            var pet = await _context.Pets.FirstOrDefaultAsync(p => p.PetId == petId && p.UserId == userId);
            if (pet == null) return false;
            _context.Pets.Remove(pet);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<PetDTO>> ListPetsAsync(long userId)
        {
            return await _context.Pets
                .Where(p => p.UserId == userId)
                .Select(p => ToPetDTO(p))
                .ToListAsync();
        }

        private static PetDTO ToPetDTO(Pet pet) => new PetDTO
        {
            PetId = pet.PetId,
            Name = pet.Name,
            Breed = pet.Breed,
            Age = pet.Age,
            Type = pet.Type,
            BehaviorComplexity = pet.BehaviorComplexity,
            HealthNotes = pet.HealthNotes,
            BehaviorNotes = pet.BehaviorNotes,
            PhotoUrl = pet.PhotoUrl,
            CreatedAt = pet.CreatedAt
        };
    }
}
