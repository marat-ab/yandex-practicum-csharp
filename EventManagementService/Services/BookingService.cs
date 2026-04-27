using EventManagementService.Exceptions;
using EventManagementService.Models;
using EventManagementService.Repository;
using EventManagementService.Repository.Extensions;

namespace EventManagementService.Services;

public class BookingService : IBookingService
{
    public readonly IBookingRepository _bookingRepository;
    public readonly ILogger<BookingService> _logger;

    public BookingService(
        IBookingRepository bookingRepository,
        ILogger<BookingService> logger)
    {
        _bookingRepository = bookingRepository;
        _logger = logger;
    }

    public async Task<Booking> CreateBookingAsync(int eventId)
    {
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
