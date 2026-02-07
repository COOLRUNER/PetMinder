using PetMinder.Shared.DTO;

namespace WebApplication1.Services.Interfaces;

public interface IGeocodingService
{
    Task<(double Lat, double Lon)?> GetCoordinatesAsync(CreateAddressDTO addressDto);
}