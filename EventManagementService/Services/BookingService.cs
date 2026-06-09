using EventManagementService.DataAccess;
using EventManagementService.Exceptions;
using EventManagementService.Models;
using EventManagementService.Models.Extensions;
using EventManagementService.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Net.NetworkInformation;

namespace EventManagementService.Services;

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

    public async Task<Booking> CreateBookingAsync(Guid eventId, CancellationToken ct = default)
    {
        try
        {
            await _bookingSemaphore.WaitAsync(ct);

            var eventTmp = await _eventService.FindEventByIdAsync(eventId, ct);

            if (eventTmp is null)
                throw new EventNotFoundException(eventId, $"Absent event with id: {eventId}");

            var isReservOk = eventTmp.TryReserveSeats();
            if (isReservOk is false)
                throw new NoAvailableSeatsException(eventId, $"No available seats for event with id {eventId}");

            await _eventService.UpdateEventAsync(eventTmp, ct);
            
            var newGuid = Guid.NewGuid();
            var createdAt = DateTime.UtcNow;
            var newBooking = new Booking(id: newGuid, 
                eventId: eventId, 
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

    public async Task<Booking> GetBookingByIdAsync(Guid bookingId, CancellationToken ct = default)
    {
        var result = await _bookingRepository.SelectBookingByIdAsync(bookingId, ct);

        return result;
    }

    public async Task<IReadOnlyList<Booking>> GetAllBookingByStatusAsync(BookingStatus status, CancellationToken ct = default)
    {
        var result = await _bookingRepository.SelectAllBookingByStatusAsync(status, ct);

        return result;
    }

    public async Task UpdateBookingAsync(Guid id, Booking newBooking, CancellationToken ct = default)
    {
        await _bookingRepository.UpdateBookingAsync(id, newBooking, ct);
    }
}
