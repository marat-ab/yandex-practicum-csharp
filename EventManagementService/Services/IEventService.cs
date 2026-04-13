using EventManagementService.Models;

namespace EventManagementService.Services;

public interface IEventService
{
    Task<IReadOnlyList<Event>> GetAllEventsAsync(
        string? title,
        DateTime? from,
        DateTime? to);

    Task<Event> GetEventByIdAsync(int id);
    Task<Event?> FindEventByIdAsync(int id);
    Task<Event> AddEventAsync(Event newEvent);
    Task UpdateEventAsync(Event eventForUpdate);
    Task RemoveEventAsync(int id);
}
