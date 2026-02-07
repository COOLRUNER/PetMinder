using Microsoft.EntityFrameworkCore;
using PetMinder.Data;
using PetMinder.Models;
using PetMinder.Shared.DTO;
using WebApplication1.Services.Interfaces;

using PetMinder.Api.Services;

namespace WebApplication1.Services;

public class BookingService : IBookingService
{
    private readonly ApplicationDbContext _context;
    private readonly INotificationService _notificationService;
    private readonly IPointsService _pointsService;
    private readonly IVerificationService _verificationService;
    private readonly IGamificationService _gamificationService;
    private readonly IReferralService _referralService;

    public BookingService(ApplicationDbContext context, INotificationService notificationService, IPointsService pointsService, IVerificationService verificationService, IGamificationService gamificationService, IReferralService referralService)
    {
        _context = context;
        _notificationService = notificationService;
        _pointsService = pointsService;
        _gamificationService = gamificationService;
        _referralService = referralService;
        _verificationService = verificationService;
    }

    private static BookingRequestDTO ToBookingRequestDTO(BookingRequest br)
    {
        var pendingChange = br.BookingChanges?
            .Where(bc => bc.ProposedStart != null && bc.ProposedEnd != null)
            .OrderByDescending(bc => bc.CreatedAt)
            .FirstOrDefault();

        return new BookingRequestDTO
        {
            BookingId = br.BookingId,
            OwnerId = br.Owner.UserId,
            OwnerFirstName = br.Owner.FirstName,
            OwnerLastName = br.Owner.LastName,
            SitterId = br.Sitter.UserId,
            SitterFirstName = br.Sitter.FirstName,
            SitterLastName = br.Sitter.LastName,
            PetId = br.Pet.PetId,
            PetName = br.Pet.Name,
            StartTime = br.StartTime,
            EndTime = br.EndTime,
            OfferedPoints = br.OfferedPoints,
            Status = br.Status,
            CreatedAt = br.CreatedAt,
            UpdatedAt = br.UpdatedAt,
            PendingChange = pendingChange != null ? new BookingChangeDTO
            {
                ChangeId = pendingChange.ChangeId,
                BookingId = pendingChange.BookingId,
                ProposedStart = pendingChange.ProposedStart,
                ProposedEnd = pendingChange.ProposedEnd,
                CreatedAt = pendingChange.CreatedAt,
                RequestedBy = pendingChange.RequestedBy
            } : null,
            HasOwnerReviewed = br.Reviews?.Any(r => r.ReviewerId == br.Owner.UserId) ?? false,
            HasSitterReviewed = br.Reviews?.Any(r => r.ReviewerId == br.Sitter.UserId) ?? false
        };
    }

    public async Task<BookingRequestDTO> CreateBookingRequestAsync(long ownerId, CreateBookingRequestDTO dto)
    {
        if (dto.StartTime >= dto.EndTime)
        {
            throw new ArgumentException("Booking end time must be after start time.");
        }
        if (dto.StartTime < DateTime.UtcNow.AddMinutes(-5))
        {
            throw new ArgumentException("Booking start time cannot be in the past.");
        }
        
        bool isVerified = await _verificationService.IsFullyVerified(ownerId);
        if (!isVerified)
        {
            throw new InvalidOperationException(
                "Account Restriction: You must verify your Email and upload a Profile Photo before you can book a sitter.");
        }

        var owner = await _context.Users.FirstOrDefaultAsync(u => u.UserId == ownerId);
        if (owner == null) throw new UnauthorizedAccessException("Owner not found.");
        if (!owner.Role.HasFlag(UserRole.Owner)) throw new UnauthorizedAccessException("Only users with Owner role can create booking requests.");

        var sitter = await _context.Users.Include(u => u.SitterSettings).FirstOrDefaultAsync(u => u.UserId == dto.SitterId);
        if (sitter == null || !sitter.Role.HasFlag(UserRole.Sitter))
        {
            throw new ArgumentException("Sitter not found or not a valid sitter.");
        }

        var pet = await _context.Pets.FirstOrDefaultAsync(p => p.PetId == dto.PetId && p.UserId == ownerId);
        if (pet == null)
        {
            throw new ArgumentException("Pet not found or does not belong to the current owner.");
        }

        var policy = await _context.PointPolicies.FirstOrDefaultAsync(p => p.ServiceType == PointPolicyServiceType.Pet);
        if (policy != null)
        {
            if (dto.OfferedPoints < policy.MinSpendable)
            {
                throw new InvalidOperationException($"The offered amount ({dto.OfferedPoints}) is too low. " +
                                                    $"The system minimum per transaction is {policy.MinSpendable} points.");
            }

            double durationHours = (dto.EndTime - dto.StartTime).TotalHours;
            int requiredBasePoints = (int)Math.Ceiling(durationHours * policy.PointsPerHour);
            
            // we allow a small discount. We just should make sure that the sitter settings is not suspiciously low 
            // to prevent fraud.
            int fraudThreshold = (int)(requiredBasePoints * 0.5);
             
            if (dto.OfferedPoints < fraudThreshold)
            {
                throw new InvalidOperationException(
                    $"The offered points are suspiciously low for a {durationHours:F1} hour booking. " +
                    $"Based on global policies, it should be closer to {requiredBasePoints}.");
            }
        }
        
        var ownerBalance = await _pointsService.GetUserPointsBalanceAsync(ownerId);
        if (ownerBalance < dto.OfferedPoints)
        {
            throw new InvalidOperationException($"Insufficient points to make this booking request. Your balance: " +
                                                $"{ownerBalance}, Offered: {dto.OfferedPoints}.");
        }

        if (sitter.SitterSettings != null && dto.OfferedPoints < sitter.SitterSettings.MinPoints)
        {
            throw new InvalidOperationException($"Offered points ({dto.OfferedPoints}) are below the sitter's minimum " +
                                                $"requirement of {sitter.SitterSettings.MinPoints}.");
        }

        bool isAvailable = await _context.SitterAvailabilities.AnyAsync(sa =>
            sa.SitterId == dto.SitterId &&
            sa.StartTime <= dto.StartTime &&
            sa.EndTime >= dto.EndTime);

        if (!isAvailable)
        {
            throw new InvalidOperationException("The sitter is not available for the exact requested time slot. " +
                                                "Please check their availability calendar.");
        }

        bool hasOverlapWithExistingBookings = await _context.BookingRequests.AnyAsync(br =>
            br.SitterId == dto.SitterId &&
            (br.Status == BookingStatus.Pending || br.Status == BookingStatus.Accepted) &&
            br.StartTime < dto.EndTime &&
            br.EndTime > dto.StartTime);

        if (hasOverlapWithExistingBookings)
        {
            throw new InvalidOperationException("The requested time slot overlaps with an existing pending or " +
                                                "accepted booking for this sitter. Double booking is not allowed.");
        }

        var bookingRequest = new BookingRequest
        {
            OwnerId = ownerId,
            SitterId = dto.SitterId,
            PetId = dto.PetId,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            OfferedPoints = dto.OfferedPoints,
            Status = BookingStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.BookingRequests.Add(bookingRequest);
        await _context.SaveChangesAsync();

        var existingConversation = await _context.Conversations
            .Include(c => c.Participants)
            .Where(c => c.Participants.Any(p => p.UserId == ownerId) && 
                       c.Participants.Any(p => p.UserId == dto.SitterId))
            .FirstOrDefaultAsync();

        if (existingConversation == null)
        {
            var conversation = new Conversation
            {
                BookingId = bookingRequest.BookingId,
                CreatedAt = DateTime.UtcNow
            };
            _context.Conversations.Add(conversation);
            await _context.SaveChangesAsync();

            _context.ConversationParticipants.Add(new ConversationParticipant { ConversationId = conversation.ConversationId, UserId = ownerId });
            _context.ConversationParticipants.Add(new ConversationParticipant { ConversationId = conversation.ConversationId, UserId = dto.SitterId });
            await _context.SaveChangesAsync();
        }
        
        string notificationContent = string.Format(
            "New booking request from {0} {1} for {2} ({3:g} - {4:g}). Offered: {5} points. Please review and respond.",
            owner.FirstName, owner.LastName, pet.Name, dto.StartTime, dto.EndTime, dto.OfferedPoints);

        await _notificationService.CreateNotificationAsync(dto.SitterId, NotificationType.BookingRequest, notificationContent, bookingRequest.BookingId);

        await _context.Entry(bookingRequest).Reference(b => b.Owner).LoadAsync();
        await _context.Entry(bookingRequest).Reference(b => b.Sitter).LoadAsync();
        await _context.Entry(bookingRequest).Reference(b => b.Pet).LoadAsync();
        await _context.Entry(bookingRequest).Collection(b => b.Reviews).LoadAsync();

        return ToBookingRequestDTO(bookingRequest);
    }

    public async Task<List<BookingRequestDTO>> GetOwnerBookingRequestsAsync(long ownerId)
    {
        var bookings = await _context.BookingRequests
            .Where(br => br.OwnerId == ownerId)
            .Include(br => br.Owner)
            .Include(br => br.Sitter)
            .Include(br => br.Pet)
            .Include(br => br.BookingChanges)
            .Include(br => br.Reviews)
            .OrderByDescending(br => br.CreatedAt)
            .ToListAsync();

        return bookings.Select(br => ToBookingRequestDTO(br)).ToList();
    }

    public async Task<List<BookingRequestDTO>> GetSitterBookingRequestsAsync(long sitterId)
    {
        var bookings = await _context.BookingRequests
            .Where(br => br.SitterId == sitterId)
            .Include(br => br.Owner)
            .Include(br => br.Sitter)
            .Include(br => br.Pet)
            .Include(br => br.BookingChanges)
            .Include(br => br.Reviews)
            .OrderByDescending(br => br.CreatedAt)
            .ToListAsync();

        return bookings.Select(br => ToBookingRequestDTO(br)).ToList();
    }

    public async Task<BookingRequestDTO> GetBookingRequestByIdAsync(long bookingId)
    {
        var booking = await _context.BookingRequests
            .Include(br => br.Owner)
            .Include(br => br.Sitter)
            .Include(br => br.Pet)
            .Include(br => br.BookingChanges)
            .Include(br => br.Reviews)
            .FirstOrDefaultAsync(br => br.BookingId == bookingId);

        return booking != null ? ToBookingRequestDTO(booking) : null;
    }

    public async Task<BookingRequestDTO> UpdateBookingStatusAsync(long bookingId, long sitterId, UpdateBookingRequestStatusDTO updateDto)
    {
        var bookingRequest = await _context.BookingRequests
            .Include(br => br.Owner)
            .Include(br => br.Sitter)
            .Include(br => br.Pet)
            .FirstOrDefaultAsync(br => br.BookingId == bookingId);

        if (bookingRequest == null)
        {
            return null;
        }

        if (bookingRequest.SitterId != sitterId)
        {
            throw new UnauthorizedAccessException("You are not authorized to update this booking request.");
        }

        if (bookingRequest.Status != BookingStatus.Pending)
        {
            throw new InvalidOperationException($"Booking request is not in a pending state and cannot be updated. " +
                                                $"Current status: {bookingRequest.Status}.");
        }

        if (IsPastDue(bookingRequest))
        {
            throw new InvalidOperationException("This booking request is past its scheduled end time and can no longer be accepted or rejected.");
        }

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            bookingRequest.Status = updateDto.NewStatus;
            bookingRequest.UpdatedAt = DateTime.UtcNow;

            string notificationContent;
            NotificationType notificationType;

            if (updateDto.NewStatus == BookingStatus.Accepted)
            {
                await _pointsService.DeductPointsAsync(
                    bookingRequest.OwnerId,
                    bookingRequest.SitterId,
                    bookingRequest.OfferedPoints,
                    TransactionType.Booking,
                    $"Points for booking request {bookingId} accepted by sitter {bookingRequest.Sitter.FirstName}."
                );

                notificationContent = $"Your booking request for {bookingRequest.Pet.Name} has been ACCEPTED " +
                                      $"by {bookingRequest.Sitter.FirstName}.";
                notificationType = NotificationType.BookingAccepted;
            }
            else if (updateDto.NewStatus == BookingStatus.Rejected)
            {
                notificationContent = $"Your booking request for {bookingRequest.Pet.Name} has been REJECTED " +
                                      $"by {bookingRequest.Sitter.FirstName}.";
                if (!string.IsNullOrWhiteSpace(updateDto.RejectionReason))
                {
                    notificationContent += $" Reason: {updateDto.RejectionReason}";
                }
                notificationType = NotificationType.BookingRejected;
            }
            else
            {
                throw new ArgumentException("Invalid status provided for booking update.");
            }

            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            await _notificationService.CreateNotificationAsync(
                bookingRequest.OwnerId,
                notificationType,
                notificationContent,
                bookingRequest.BookingId
            );

            return ToBookingRequestDTO(bookingRequest);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<BookingChange> ProposeChangeAsync(long userId, UpdateBookingChangeDTO bookingChangeDto)
    {
        if (bookingChangeDto.ProposedStartTime >= bookingChangeDto.ProposedEndTime)
        {
            throw new ArgumentException("Proposed start time cannot be later or the same time than proposed end time.");
        }

        var booking =
            await _context.BookingRequests.FirstOrDefaultAsync(br => br.BookingId == bookingChangeDto.BookingId);

        if (booking == null)
        {
            throw new KeyNotFoundException($"Booking {bookingChangeDto.BookingId} is null.");
        }

        if (booking.Status != BookingStatus.Accepted)
        {
            throw new InvalidOperationException("Booking change can be requested only for ACCEPTED bookings.");
        }

        if (IsPastDue(booking))
        {
            throw new InvalidOperationException("Cannot propose changes for a booking that is past its scheduled end time.");
        }

        if (userId != booking.OwnerId && userId != booking.SitterId)
        {
            throw new UnauthorizedAccessException("Only participants can propose a change.");
        }

        var change = new BookingChange
        {
            BookingId = bookingChangeDto.BookingId,
            ProposedStart = bookingChangeDto.ProposedStartTime,
            ProposedEnd = bookingChangeDto.ProposedEndTime,
            CreatedAt = DateTime.UtcNow,
            RequestedBy = userId == booking.OwnerId ? ChangeRequestedBy.Owner : ChangeRequestedBy.Sitter
        };

        _context.BookingChanges.Add(change);
        await _context.SaveChangesAsync();

        var receiverId = userId == booking.OwnerId ? booking.SitterId : booking.OwnerId;
        await _notificationService.CreateNotificationAsync(receiverId,
            NotificationType.ChangeRequested,
            $"Change has been requested for booking {booking.BookingId}",
            booking.BookingId);

        return change;
    }

    public async Task<BookingRequest> AcceptChangeAsync(long userId, BookingChangeAcceptDTO changeAcceptDto)
    {
        var change = await _context.BookingChanges.Include(bc => bc.BookingRequest)
            .FirstOrDefaultAsync(bc => bc.ChangeId == changeAcceptDto.ChangeId);

        if (change == null)
        {
            throw new KeyNotFoundException("Booking change is null.");
        }

        var booking = change.BookingRequest;

        if (booking == null)
        {
            throw new KeyNotFoundException("Booking for this change is null.");
        }

        bool isAcceptorSitter = userId == booking.SitterId;
        bool isAcceptorOwner = userId == booking.OwnerId;

        if (!isAcceptorSitter && !isAcceptorOwner)
        {
            throw new UnauthorizedAccessException("User is not a participant in this booking.");
        }

        if ((change.RequestedBy == ChangeRequestedBy.Owner && !isAcceptorSitter) ||
             (change.RequestedBy == ChangeRequestedBy.Sitter && !isAcceptorOwner))
        {
            throw new UnauthorizedAccessException("You cannot accept your own proposed changes. Only the other participant can accept.");
        }

        if (change.ProposedStart == null || change.ProposedEnd == null)
        {
            throw new InvalidOperationException("Change is invalid or already has been accepted.");
        }

        booking.StartTime = change.ProposedStart.Value;
        booking.EndTime = change.ProposedEnd.Value;
        booking.UpdatedAt = DateTime.UtcNow;

        change.ProposedStart = null;
        change.ProposedEnd = null;

        _context.BookingRequests.Update(booking);
        _context.BookingChanges.Update(change);
        await _context.SaveChangesAsync();

        var senderId = userId == booking.OwnerId ? booking.SitterId : booking.OwnerId;
        await _notificationService.CreateNotificationAsync(senderId,
            NotificationType.ChangeAccepted,
            $"Your change proposal for booking {booking.BookingId} has been ACCEPTED.",
            booking.BookingId);

        return booking;
    }

    private bool IsPastDue(BookingRequest booking)
    {
        return booking.Status != BookingStatus.Completed &&
               booking.Status != BookingStatus.Cancelled &&
               booking.EndTime < DateTime.UtcNow;
    }

    public async Task<BookingCancellation> CancelBookingAsync(long userId, CreateCancellationDTO createCancellationDto)
    {
        using var tx = await _context.Database.BeginTransactionAsync();

        try
        {
            var booking = await _context.BookingRequests
                .FirstOrDefaultAsync(br => br.BookingId == createCancellationDto.BookingId);

            if (booking == null)
            {
                throw new KeyNotFoundException($"Booking no {createCancellationDto.BookingId} not found.");
            }

            if (booking.Status != BookingStatus.Accepted)
            {
                throw new InvalidOperationException(
                    $"Only accepted bookings can be cancelled. Current Status: {booking.Status}");
            }

            if (IsPastDue(booking))
            {
                throw new InvalidOperationException("Cannot cancel a booking that is past its scheduled end time.");
            }

            if (userId != booking.OwnerId && userId != booking.SitterId)
            {
                throw new UnauthorizedAccessException("Only participants can cancel a booking.");
            }

            const int lateCancellationHours = 48;
            const long standardPolicyId = 1; 
            const long latePolicyId = 2; 
            const long inProgressPolicyId = 3; 

            var timeUntilStart = booking.StartTime.Subtract(DateTime.UtcNow);

            long chosenPolicyId;

            if (timeUntilStart.TotalHours <= 0)
            {
                chosenPolicyId = inProgressPolicyId;
            }
            else if (timeUntilStart.TotalHours < lateCancellationHours)
            {
                chosenPolicyId = latePolicyId;
            }
            else
            {
                chosenPolicyId = standardPolicyId;
            }
            var policy = await _context.CancellationPolicies
                .FirstOrDefaultAsync(cp => cp.PolicyId == chosenPolicyId);

            if (policy == null)
            {
                throw new InvalidOperationException($"Required cancellation policy {chosenPolicyId} not found.");
            }

            var cancelledBy = (userId == booking.OwnerId) ? CancelledBy.Owner : CancelledBy.Sitter;
            var refundPercentage = policy.RefundPercentage;

            var refundAmount = (int)Math.Floor(booking.OfferedPoints * (refundPercentage / 100.0));

            booking.Status = BookingStatus.Cancelled;
            booking.UpdatedAt = DateTime.UtcNow;
            _context.BookingRequests.Update(booking);

            var cancellationRecord = new BookingCancellation
            {
                BookingId = createCancellationDto.BookingId,
                PolicyId = chosenPolicyId,
                CancelledBy = cancelledBy,
                CancelledAt = DateTime.UtcNow
            };
            _context.BookingCancellations.Add(cancellationRecord);

            if (refundAmount > 0)
            {
                const long systemUserId = -1;

                await _pointsService.CreditPointsAsync(
                    receiverId: booking.OwnerId,
                    senderId: systemUserId,
                    points: refundAmount,
                    type: TransactionType.Adjustment,
                    reason: $"Cancellation refund ({refundPercentage}%) for booking {booking.BookingId}"
                );
            }

            await _context.SaveChangesAsync();
            await tx.CommitAsync();

            var receiverId = (userId == booking.OwnerId) ? booking.SitterId : booking.OwnerId;
            var senderRole = (userId == booking.OwnerId) ? "Owner" : "Sitter";

            await _notificationService.CreateNotificationAsync(
                userId: receiverId,
                notificationType: NotificationType.BookingCancelled,
                content: $"Booking {booking.BookingId} has been CANCELLED by the {senderRole}. " +
                         $"Refund of {refundPercentage}% has been processed.",
                bookingId: booking.BookingId
            );

            return cancellationRecord;
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync();
            throw;
        }
    }
    public async Task<BookingRequestDTO> CompleteBookingAsync(long userId, long bookingId)
    {
        using var tx = await _context.Database.BeginTransactionAsync();
        try
        {
            var booking = await _context.BookingRequests
                .Include(b => b.Owner)
                .Include(b => b.Sitter)
                .Include(b => b.Pet)
                .Include(b => b.Reviews)
                .FirstOrDefaultAsync(b => b.BookingId == bookingId);

            if (booking == null) throw new KeyNotFoundException("Booking not found.");

            if (booking.OwnerId != userId)
                throw new UnauthorizedAccessException("Only the owner can mark a booking as completed.");

            if (booking.Status != BookingStatus.Accepted)
                throw new InvalidOperationException($"Booking must be in Accepted status to complete. Current: {booking.Status}");

            booking.Status = BookingStatus.Completed;
            booking.UpdatedAt = DateTime.UtcNow;
            _context.BookingRequests.Update(booking);

            await _pointsService.CreditPointsAsync(
                booking.SitterId,
                booking.OwnerId,
                booking.OfferedPoints,
                TransactionType.Booking,
                $"Points released for completed booking {booking.BookingId}"
            );

            await _context.SaveChangesAsync();
            await tx.CommitAsync();

            await _notificationService.CreateNotificationAsync(
                booking.SitterId,
                NotificationType.BookingCompleted, 
                $"Booking for {booking.Pet.Name} has been marked as COMPLETED by {booking.Owner.FirstName}.",
                booking.BookingId
            );

            await _gamificationService.CheckAndAwardBadgesAsync(booking.SitterId);
            await _referralService.CompleteReferralAsync(booking.OwnerId);
            await _gamificationService.CheckAndAwardBadgesAsync(booking.OwnerId);

            return ToBookingRequestDTO(booking);
        }
        catch (Exception)
        {
            await tx.RollbackAsync();
            throw;
        }
    }
}