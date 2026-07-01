using EventManagementService.Models;

namespace EventManagementService.Repositories;

public interface IBookingRepository
{
    Task<Booking> InsertBookingAsync(Booking newBooking, CancellationToken ct = default);

    Task<Booking> SelectBookingByIdAsync(Guid bookingId, CancellationToken ct = default);
    Task<IReadOnlyList<Booking>> SelectAllBookingByStatusAsync(BookingStatus status, CancellationToken ct = default);

    Task UpdateBookingAsync(Guid id, Booking newBooking, CancellationToken ct = default);
}
