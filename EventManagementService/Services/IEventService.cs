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

    Task<Event> GetEventByIdAsync(int id);
    Task<Event?> FindEventByIdAsync(int id);
    Task<Event> AddEventAsync(Event newEvent);
    Task UpdateEventAsync(Event eventForUpdate);
    Task RemoveEventAsync(int id);
}
