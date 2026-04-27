using EventManagementService.Exceptions;
using EventManagementService.Models;
using EventManagementService.Repository.Models;

namespace EventManagementService.Repository;

internal sealed class BookingRepository : IBookingRepository
{
    private readonly Dictionary<Guid, BookingEntity> _bookings = new();

    // Т.к. события берутся не из репозитория, а из Dictionary
    // на всякий случай обращение с ним сделал в рамках lock'а
    // Альтернативный вариант - использовать ConcurrentDictionary
    private object _lock = new object();

    public BookingRepository(CancellationToken ct = default)
    {
        
    }

    public Task<IReadOnlyList<BookingEntity>> SelectAllBookingAsync(CancellationToken ct = default)
    {
        lock(_lock)
        {
            var result = _bookings
                .Select(kv => kv.Value)
                .ToList();

            return Task.FromResult((IReadOnlyList<BookingEntity>)result);
        }
    }

    public Task<BookingEntity> SelectBookingByIdAsync(Guid id, CancellationToken ct = default)
    {
        lock (_lock)
        {
            if (_bookings.TryGetValue(id, out BookingEntity? value))
            {
                return Task.FromResult<BookingEntity>(value);
            }
            else
            {
                throw new BookingNotFoundException(id,
                    $"Can't get booking with id = {id}. It is absent");
            }
        }
    }

    public Task InsertBooking(BookingEntity entity, CancellationToken ct = default)
    {
        lock (_lock)
        {            
            _bookings[entity.Id] = entity;

            return Task.CompletedTask;
        }
    }
}
