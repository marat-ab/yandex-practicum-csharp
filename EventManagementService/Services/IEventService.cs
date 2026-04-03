using EventManagementService.Models;

namespace EventManagementService.Services;

public interface IEventService
{
    public Task<IReadOnlyList<Event>> GetAllEventsAsync();
    public Task<Event?> FindEventByIdAsync(int id);
    public Task<Event> AddEventAsync(Event newEvent);
    public Task UpdateEventAsync(Event eventForUpdate);
    public Task RemoveEventAsync(int id);
}
