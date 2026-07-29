using Bookings.Domain.Models;
using Bookings.Domain.Models.Auth;

namespace Bookings.Application.Services;

public interface IBookingService
{
    Task<Booking> CreateBookingAsync(Guid eventId, Guid userId, CancellationToken ct = default);

    Task CancelBookingAsync(Guid bookingId, Guid userId, Role role, CancellationToken ct = default);

    Task<Booking> GetBookingByIdAsync(Guid bookingId, Guid userId, CancellationToken ct = default);
    Task<IReadOnlyList<Booking>> GetAllBookingByStatusAsync(BookingStatus status, CancellationToken ct = default);

    Task UpdateBookingAsync(Guid bookingId, Booking newBooking, CancellationToken ct = default);
}
