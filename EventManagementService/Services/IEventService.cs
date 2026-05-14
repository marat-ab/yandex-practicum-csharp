using EventManagementService.Models;

namespace EventManagementService.Services;

public interface IEventService
{
    Task<PaginatedResult> GetAllEventsAsync(
        string? title = null,
        DateTime? from = null,
        DateTime? to = null,
        int? pageNumber = null,
        int? pageSize = null);

    Task<Event> GetEventByIdAsync(Guid id);

    Task<Event?> FindEventByIdAsync(Guid id);

    Event? FindEventById(Guid id);

    Task<Event> AddEventAsync(Event newEvent);

    Task UpdateEventAsync(Event eventForUpdate);

    void UpdateEvent(Event eventForUpdate);

    Task RemoveEventAsync(Guid id);
}
