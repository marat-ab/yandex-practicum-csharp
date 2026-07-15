using EventManagementService.Domain.Models;

namespace EventManagementService.Application.Repositories;

public interface IBookingRepository
{
    Task<Booking> InsertBookingAsync(Booking newBooking, CancellationToken ct = default);

    Task<Booking> SelectBookingByIdAsync(Guid bookingId, CancellationToken ct = default);
    Task<IReadOnlyList<Booking>> SelectAllActiveBookingForUserAsync(Guid userId, CancellationToken ct = default);
    Task<IReadOnlyList<Booking>> SelectAllBookingByStatusAsync(BookingStatus status, CancellationToken ct = default);

    Task UpdateBookingAsync(Guid bookingId, Booking newBooking, CancellationToken ct = default);
}
