using PetMinder.Models;
using PetMinder.Shared.DTO;

namespace WebApplication1.Services.Interfaces;

public interface IBookingService
{
    Task<BookingRequestDTO> CreateBookingRequestAsync(long ownerId, CreateBookingRequestDTO dto);
    Task<List<BookingRequestDTO>> GetOwnerBookingRequestsAsync(long ownerId);
    Task<List<BookingRequestDTO>> GetSitterBookingRequestsAsync(long sitterId);
    Task<BookingRequestDTO> GetBookingRequestByIdAsync(long bookingId);
    Task<BookingRequestDTO> UpdateBookingStatusAsync(long bookingId, long sitterId,
        UpdateBookingRequestStatusDTO updateDto);
    Task<BookingChange> ProposeChangeAsync(long userId, UpdateBookingChangeDTO bookingChangeDto);
    Task<BookingRequest> AcceptChangeAsync(long userId, BookingChangeAcceptDTO changeAcceptDto);
    Task<BookingCancellation> CancelBookingAsync(long userId, CreateCancellationDTO createCancellationDto);
    Task<BookingRequestDTO> CompleteBookingAsync(long userId, long bookingId);
}