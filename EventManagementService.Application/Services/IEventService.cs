using EventManagementService.Domain.Models;
using EventManagementService.Models;

namespace EventManagementService.Application.Services;

public interface IEventService
{
    Task<PaginatedResult> GetAllEventsAsync(
        string? title = null,
        DateTime? from = null,
        DateTime? to = null,
        int? pageNumber = null,
        int? pageSize = null,
        CancellationToken ct = default);

    Task<Event> GetEventByIdAsync(Guid id, CancellationToken ct = default);

    Task<Event?> FindEventByIdAsync(Guid id, CancellationToken ct = default);

    Task<Event> AddEventAsync(Event newEvent, CancellationToken ct = default);

    Task UpdateEventAsync(Event eventForUpdate, CancellationToken ct = default);

    Task RemoveEventAsync(Guid id, CancellationToken ct = default);
}
