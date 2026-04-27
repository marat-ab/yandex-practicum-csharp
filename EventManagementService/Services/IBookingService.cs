using EventManagementService.Models;

namespace EventManagementService.Services;

public interface IBookingService
{
    Task<Booking> CreateBookingAsync(int eventId);

    Task<Booking> GetBookingByIdAsync(Guid bookingId);
}
