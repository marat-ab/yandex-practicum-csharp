using EventManagementService.Models;
using EventManagementService.Models.Entities;

namespace EventManagementService.Services;

public interface IBookingService
{
    Task<Booking> CreateBookingAsync(Guid eventId, CancellationToken ct = default);

    Task<Booking> GetBookingByIdAsync(Guid bookingId, CancellationToken ct = default);
    Task<IReadOnlyList<Booking>> GetAllBookingByStatusAsync(BookingStatus status, CancellationToken ct = default);

    Task UpdateBookingAsync(Guid id, Booking newBooking, CancellationToken ct = default);
}
