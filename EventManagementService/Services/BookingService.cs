using EventManagementService.Exceptions;
using EventManagementService.Models;
using EventManagementService.Repository;
using EventManagementService.Repository.Extensions;

namespace EventManagementService.Services;

public class BookingService : IBookingService
{
    public readonly IBookingRepository _bookingRepository;

    public BookingService(IBookingRepository bookingRepository)
    {
        _bookingRepository = bookingRepository;
    }

    public async Task<Booking> CreateBookingAsync(Guid eventId)
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
