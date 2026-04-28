using EventManagementService.Models;

namespace EventManagementService.Repository;

public interface IBookingRepository
{
    Task<IReadOnlyList<BookingEntity>> SelectAllBookingAsync(CancellationToken ct = default);
    Task<BookingEntity> SelectBookingByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<BookingEntity>> SelectAllBookingByStatusAsync(BookingStatus status, CancellationToken ct = default);

    Task InsertBookingAsync(BookingEntity entity, CancellationToken ct = default);

    Task UpdateBookingAsync(Guid id, BookingEntity newBooking, CancellationToken ct = default);
}
