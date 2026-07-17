using EventManagementService.Application.Repositories;
using EventManagementService.Domain.Exceptions;
using EventManagementService.Domain.Models;
using EventManagementService.Domain.Models.Auth;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace EventManagementService.Application.Services;

public class BookingService : IBookingService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IEventService _eventService;

    private static readonly SemaphoreSlim _bookingSemaphore = new(1, 1);

    public BookingService(
        IBookingRepository bookingRepository,
        IEventService eventService)
    {
        _bookingRepository = bookingRepository;
        _eventService = eventService;
    }

    public async Task<Booking> CreateBookingAsync(Guid eventId, Guid userId, CancellationToken ct = default)
    {
        try
        {
            await _bookingSemaphore.WaitAsync(ct);

            var eventTmp = await _eventService.FindEventByIdAsync(eventId, ct);

            if (eventTmp is null)
                throw new EventNotFoundException(eventId, $"Absent event with id: {eventId}");

            var currentDt = DateTime.UtcNow;
            if (currentDt >= eventTmp.StartAt)
                throw new EventAlreadyStartedException(eventId, $"Event with id {eventId} is already started");

            var activeBookingsForUser = await _bookingRepository.SelectAllActiveBookingForUserAsync(userId, ct);
            if (activeBookingsForUser.Count >= 10)
                throw new BookingUserOverflowException(userId, $"Booking for user with id {userId} is overflowed");

            var isReservOk = eventTmp.TryReserveSeats();
            if (isReservOk is false)
                throw new NoAvailableSeatsException(eventId, $"No available seats for event with id {eventId}");            

            await _eventService.UpdateEventAsync(eventTmp, ct);

            var newGuid = Guid.NewGuid();
            var createdAt = DateTime.UtcNow;
            var newBooking = new Booking(id: newGuid,
                eventId: eventId,
                userId: userId,
                status: BookingStatus.Pending,
                createdAt: createdAt);

            await _bookingRepository.InsertBookingAsync(newBooking, ct);

            return newBooking;
        }
        finally
        {
            _bookingSemaphore.Release();
        }
    }

    public async Task CancelBookingAsync(Guid bookingId, Guid userId, Role role, CancellationToken ct = default)
    {
        var booking = await _bookingRepository.SelectBookingByIdAsync(bookingId, ct);

        if (booking is null)
            throw new BookingNotFoundException(bookingId, $"Booking with id = {bookingId} not found)");

        if (role != Role.Admin)
        {
            if (booking.UserId != userId)
                throw new BookingAccessDeniedException(userId, $"User with id {userId} can't cancel booking " +
                    $"with id {bookingId}. No access to this booking.");
        }

        if (booking.Status == BookingStatus.Cancelled)
            throw new BookingAlreadyCancelledException(bookingId, $"Booking with id = {bookingId} already cancelled");

        booking.Cancel();

        await UpdateBookingAsync(bookingId, booking, ct);
    }

    public async Task<Booking> GetBookingByIdAsync(Guid bookingId, Guid userId, CancellationToken ct = default)
    {
        var booking = await _bookingRepository.SelectBookingByIdAsync(bookingId, ct);

        if (booking is null)
            throw new BookingNotFoundException(bookingId, $"Booking with id = {bookingId} not found");

        if (booking.UserId != userId)
            throw new BookingAccessDeniedException(bookingId, $"Booking with id = {bookingId} access denied");

        return booking;
    }

    public async Task<IReadOnlyList<Booking>> GetAllBookingByStatusAsync(BookingStatus status, CancellationToken ct = default)
    {
        var result = await _bookingRepository.SelectAllBookingByStatusAsync(status, ct);

        return result;
    }

    public async Task UpdateBookingAsync(Guid bookingId, Booking newBooking, CancellationToken ct = default)
    {
        await _bookingRepository.UpdateBookingAsync(bookingId, newBooking, ct);
    }
}
