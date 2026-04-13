using EventManagementService.Models;

namespace EventManagementService.Services;

public interface IEventService
{
    Task<PaginatedResult> GetAllEventsAsync(
        string? title,
        DateTime? from,
        DateTime? to,
        int pageNumber = 1,
        int pageSize = 10);

    Task<Event> GetEventByIdAsync(int id);
    Task<Event?> FindEventByIdAsync(int id);
    Task<Event> AddEventAsync(Event newEvent);
    Task UpdateEventAsync(Event eventForUpdate);
    Task RemoveEventAsync(int id);
}
