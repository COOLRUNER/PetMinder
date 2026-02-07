using Microsoft.EntityFrameworkCore;
using PetMinder.Data;
using PetMinder.Models;
using PetMinder.Shared.DTO;
using WebApplication1.Services.Interfaces;

namespace WebApplication1.Services;

public class AddressService : IAddressService
{
    private readonly ApplicationDbContext _context;
    private readonly IGeocodingService _geocodingService;
    private readonly ILogger<AddressService> _logger;
    
    public AddressService(ApplicationDbContext context, IGeocodingService geocodingService, ILogger<AddressService> logger)
    {
        _context = context;
        _geocodingService = geocodingService;
        _logger = logger;
    }

    public async Task<AddressDTO> AddAddressAsync(long userId, CreateAddressDTO dto)
    {
        var coordinates = await _geocodingService.GetCoordinatesAsync(dto);
        if (coordinates == null)
        {
            throw new InvalidOperationException("Could not find coordinates for this address. Please check the street, city, and zip code.");
        }
        
        var (lat, lon) = coordinates.Value;
        
        var address = await _context.Addresses
            .FirstOrDefaultAsync(a => a.Street == dto.Street && a.City == dto.City && a.ZipCode == dto.ZipCode);
        
        if (address == null)
        {
            address = new Address
            {
                Street = dto.Street,
                City = dto.City,
                ZipCode = dto.ZipCode,
                Country = "Poland",
                Latitude = lat,
                Longitude = lon
            };
            _context.Addresses.Add(address);
            await _context.SaveChangesAsync();
        }
        
        if (await _context.UserAddresses.AnyAsync(ua => ua.UserId == userId && ua.AddressId == address.AddressId))
        {
            throw new InvalidOperationException("You have already added this address to your profile.");
        }
        
        if (dto.Type == AddressType.Primary)
        {
            var otherPrimaryAddresses = await _context.UserAddresses
                .Where(ua => ua.UserId == userId && ua.Type == AddressType.Primary)
                .ToListAsync();
                
            foreach (var oldPrimary in otherPrimaryAddresses)
            {
                oldPrimary.Type = AddressType.Secondary;
            }
        }
        
        var userAddress = new UserAddress
        {
            UserId = userId,
            AddressId = address.AddressId,
            Type = dto.Type
        };
        
        _context.UserAddresses.Add(userAddress);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("User {UserId} added new address {AddressId}", userId, address.AddressId);
        
        return new AddressDTO
        {
            UserAddressId = userAddress.UserAddressId,
            AddressId = address.AddressId,
            Street = address.Street,
            City = address.City,
            Country = address.Country,
            ZipCode = address.ZipCode,
            Type = userAddress.Type,
            Latitude = address.Latitude,
            Longitude = address.Longitude
        };
    }

    public async Task<List<AddressDTO>> GetUserAddressesAsync(long userId)
    {
        return await _context.UserAddresses
            .Where(ua => ua.UserId == userId)
            .Include(ua => ua.Address)
            .Select(ua => new AddressDTO
            {
                UserAddressId = ua.UserAddressId,
                AddressId = ua.AddressId,
                Street = ua.Address.Street,
                City = ua.Address.City,
                Country = ua.Address.Country,
                ZipCode = ua.Address.ZipCode,
                Type = ua.Type,
                Latitude = ua.Address.Latitude,
                Longitude = ua.Address.Longitude
            }).ToListAsync();
    }

    public async Task<bool> DeleteUserAddressAsync(long userId, long userAddressId)
    {
        var userAddress = await _context.UserAddresses
            .FirstOrDefaultAsync(ua => ua.UserId == userId && ua.UserAddressId == userAddressId);
        
        if (userAddress == null)
        {
            return false;
        }
        
        _context.UserAddresses.Remove(userAddress);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("User {UserId} removed UserAddress {UserAddressId}", userId, userAddressId);
        return true;
    }
}