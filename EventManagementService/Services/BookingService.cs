using EventManagementService.Exceptions;
using EventManagementService.Models;
using EventManagementService.Models.Extensions;
using EventManagementService.Repository;

namespace EventManagementService.Services;

public class BookingService : IBookingService
{
    public readonly IBookingRepository _bookingRepository;
    public readonly IEventService _eventService;

    public BookingService(
        IBookingRepository bookingRepository,
        IEventService eventService)
    {
        _bookingRepository = bookingRepository;
        _eventService = eventService;
    }

    public async Task<Booking> CreateBookingAsync(Guid eventId)
    {
        var eventTmp = await _eventService.FindEventByIdAsync(eventId);

        if (eventTmp is null)
            throw new EventNotFoundException(eventId, $"Absent event with id: {eventId}");

        var newGuid = Guid.NewGuid();
        var createdAt = DateTime.UtcNow;
        var newBooking = new Booking(Id: newGuid, EventId: eventId, Status: BookingStatus.Pending, CreatedAt: createdAt);

        var bookingForStore = newBooking.ToBookingEntity();

        await _bookingRepository.InsertBookingAsync(bookingForStore);

        return newBooking;
    }

    public async Task<Booking> GetBookingByIdAsync(Guid bookingId)
    {
        var bookingEntity = await _bookingRepository.SelectBookingByIdAsync(bookingId);

        var result = bookingEntity.ToBooking();

        return result;
    }
}
