using EventManagementService.Exceptions;
using EventManagementService.Models;

namespace EventManagementService.Services;

public class BookingService : IBookingService
{
    public readonly ILogger<BookingService> _logger;

    private readonly Dictionary<Guid, Booking> _bookings = new();

    // Т.к. события берутся не из репозитория, а из Dictionary
    // на всякий случай обращение с ним сделал в рамках lock'а
    // Альтернативный вариант - использовать ConcurrentDictionary
    private object _lock = new object();

    public BookingService(ILogger<BookingService> logger)
    {
        _logger = logger;
    }

    public Task CreateBookingAsync(int eventId)
    {
        lock (_lock)
        {
            var newGuid = Guid.NewGuid();
            var createdAt = DateTime.UtcNow;
            var newBooking = new Booking(Id: newGuid, EventId: eventId, Status: BookingStatus.Pending, CreatedAt: createdAt);

            _bookings[newGuid] = newBooking;

            return Task.CompletedTask;
        }
    }

    public Task GetBookingByIdAsync(Guid bookingId)
    {
        lock (_lock)
        {
            if (_bookings.TryGetValue(bookingId, out Booking? value))
            {
                return Task.FromResult<Booking>(value);
            }
            else
            {
                throw new BookingNotFoundException(bookingId,
                    $"Can't get booking with id = {bookingId}. It is absent");
            }
        }
    }
}
