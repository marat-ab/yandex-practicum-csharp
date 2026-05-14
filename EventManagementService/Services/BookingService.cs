using EventManagementService.Exceptions;
using EventManagementService.Models;
using EventManagementService.Models.Extensions;
using EventManagementService.Repository;

namespace EventManagementService.Services;

public class BookingService : IBookingService
{
    public readonly IBookingRepository _bookingRepository;
    public readonly IEventService _eventService;

    private readonly object _bookingLock = new();

    public BookingService(
        IBookingRepository bookingRepository,
        IEventService eventService)
    {
        _bookingRepository = bookingRepository;
        _eventService = eventService;
    }

    public async Task<Booking> CreateBookingAsync(Guid eventId, CancellationToken ct = default)
    {
        lock (_bookingLock)
        {
            var eventTmp = _eventService.FindEventById(eventId);

            if (eventTmp is null)
                throw new EventNotFoundException(eventId, $"Absent event with id: {eventId}");

            var isReservOk = eventTmp.TryReserveSeats();
            if (isReservOk is false)
                throw new NoAvailableSeatsException(eventId, $"No available seats for event with id {eventId}");

            _eventService.UpdateEvent(eventTmp);

            var newGuid = Guid.NewGuid();
            var createdAt = DateTime.UtcNow;
            var newBooking = new Booking(Id: newGuid, EventId: eventId, Status: BookingStatus.Pending, CreatedAt: createdAt);

            var bookingForStore = newBooking.ToBookingEntity();

            _bookingRepository.InsertBooking(bookingForStore);

            return newBooking;
        }
    }

    public async Task<Booking> GetBookingByIdAsync(Guid bookingId, CancellationToken ct = default)
    {
        var bookingEntity = await _bookingRepository.SelectBookingByIdAsync(bookingId, ct);

        var result = bookingEntity.ToBooking();

        return result;
    }

    public async Task<IReadOnlyList<Booking>> GetAllBookingByStatusAsync(BookingStatus status, CancellationToken ct = default)
    {
        var bookingEntityes = await _bookingRepository.SelectAllBookingByStatusAsync(status, ct);

        var result = bookingEntityes
            .Select(x => x.ToBooking())
            .ToList();

        return result;
    }

    public async Task UpdateBookingAsync(Guid id, Booking newBooking, CancellationToken ct = default)
    {
        var newBookingEntity = newBooking.ToBookingEntity();
        await _bookingRepository.UpdateBookingAsync(id, newBookingEntity, ct);
    }
}
