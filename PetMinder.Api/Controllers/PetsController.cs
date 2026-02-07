using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using PetMinder.Data;
using PetMinder.Models;
using PetMinder.Shared.DTO;
using WebApplication1.Services.Interfaces;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PetsController : ControllerBase
    {
        private readonly IPetService _petService;
        private readonly ApplicationDbContext _context;
        public PetsController(IPetService petService, ApplicationDbContext context)
        {
            _petService = petService;
            _context = context;
        }

        private long GetUserId() => long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

        [HttpGet]
        public async Task<IActionResult> ListPets()
        {
            var userId = GetUserId();
            var pets = await _petService.ListPetsAsync(userId);
            return Ok(pets);
        }

        [HttpPost]
        public async Task<IActionResult> AddPet([FromBody] CreatePetDTO dto)
        {
            var userId = GetUserId();
            try
            {
                var pet = await _petService.AddPetAsync(userId, dto);
                //Have to add token refresh and maybe add role sitter
                return Ok(pet);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error adding pet.", details = ex.Message });
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdatePet([FromBody] UpdatePetDTO dto)
        {
            var userId = GetUserId();
            try
            {
                var pet = await _petService.UpdatePetAsync(userId, dto);
                return Ok(pet);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating pet.", details = ex.Message });
            }
        }

        [HttpDelete("{petId}")]
        public async Task<IActionResult> DeletePet(long petId)
        {
            var userId = GetUserId();
            var result = await _petService.DeletePetAsync(userId, petId);
            if (!result) return NotFound("Pet not found or not owned by user.");
            return NoContent();
        }
    }
}
