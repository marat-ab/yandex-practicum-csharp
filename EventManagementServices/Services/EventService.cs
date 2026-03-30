using EventManagementServices.Models;

namespace EventManagementServices.Services;

public class EventService : IEventService
{
    private int _lastId = 1;
    private readonly Dictionary<int, Event> _events = [];

    public Task<IReadOnlyList<Event>> GetAllEventsAsync()
    {
        var result = _events.Values.ToList();

        return Task.FromResult((IReadOnlyList<Event>)result);
    }

    public Task<Event?> FindEventByIdAsync(int id)
    {
        if(_events.TryGetValue(id, out Event? value))
        {
            return Task.FromResult<Event?>(value);
        }
        else
        {
            return Task.FromResult<Event?>(null);
        }
    }

    public Task AddEventAsync(Event newEvent)
    {
        var id = Interlocked.Increment(ref _lastId);
        var tmpEvent = new Event()
        {
            Id = id,
            Title = newEvent.Title,
            Description = newEvent.Description,
            StartAt = newEvent.StartAt,
            EndAt = newEvent.EndAt,
        };
        _events.Add(id, tmpEvent);

        return Task.CompletedTask;
    }

    public Task UpdateEventAsync(Event eventForUpdate)
    {
        if (_events.ContainsKey(eventForUpdate.Id) is false)
            throw new KeyNotFoundException($"Event with id = {eventForUpdate.Id} is absent");

        _events[eventForUpdate.Id] = eventForUpdate;

        return Task.CompletedTask;
    }

    public Task RemoveEventAsync(int id)
    {
        if (_events.ContainsKey(id) is false)
            throw new KeyNotFoundException($"Event with id = {id} is absent");

        _events.Remove(id);

        return Task.CompletedTask;
    }
}
