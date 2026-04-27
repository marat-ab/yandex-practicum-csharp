using EventManagementService.Repository.Models;

namespace EventManagementService.Repository;

public interface IBookingRepository
{
    Task<IReadOnlyList<BookingEntity>> SelectAllBookingAsync(CancellationToken ct = default);
    Task<BookingEntity> SelectBookingByIdAsync(Guid id, CancellationToken ct = default);
    Task InsertBooking(BookingEntity entity, CancellationToken ct = default);
}
