using EventManagementService.Domain.Models;

namespace EventManagementService.Application.Services;

public interface IBookingService
{
    Task<Booking> CreateBookingAsync(Guid eventId, long userId, CancellationToken ct = default);

    Task CancelBookingAsync(Guid eventId, CancellationToken ct = default);

    Task<Booking> GetBookingByIdAsync(Guid bookingId, CancellationToken ct = default);
    Task<IReadOnlyList<Booking>> GetAllBookingByStatusAsync(BookingStatus status, CancellationToken ct = default);

    Task UpdateBookingAsync(Guid bookingId, Booking newBooking, CancellationToken ct = default);
}
