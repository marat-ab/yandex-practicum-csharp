using EventManagementService.Domain.Models;

namespace EventManagementService.Application.Services;

public interface IBookingService
{
    Task<Booking> CreateBookingAsync(Guid eventId, Guid userId, CancellationToken ct = default);

    Task CancelBookingAsync(Guid eventId, Guid userId, CancellationToken ct = default);

    Task<Booking> GetBookingByIdAsync(Guid bookingId, CancellationToken ct = default);
    Task<IReadOnlyList<Booking>> GetAllBookingByStatusAsync(BookingStatus status, CancellationToken ct = default);

    Task UpdateBookingAsync(Guid bookingId, Booking newBooking, CancellationToken ct = default);
}
