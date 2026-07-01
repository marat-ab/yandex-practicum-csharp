using EventManagementService.Models;

namespace EventManagementService.Repositories;

public interface IEventRepository
{
    Task<PaginatedResult> SelectAllEventsAsync(
        string? title = null,
        DateTime? from = null,
        DateTime? to = null,
        int? pageNumber = null,
        int? pageSize = null,
        CancellationToken ct = default);

    Task<Event> SelectEventByIdAsync(Guid id, CancellationToken ct = default);

    Task<Event> InsertEventAsync(Event newEvent, CancellationToken ct = default);

    Task UpdateEventAsync(Event eventValue, CancellationToken ct = default);

    Task DeleteEventAsync(Guid id, CancellationToken ct = default);
}
