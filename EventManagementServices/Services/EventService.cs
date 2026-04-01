using EventManagementServices.Models;

namespace EventManagementServices.Services;

public class EventService : IEventService
{
    private int _lastId = 0;
    private readonly Dictionary<int, Event> _events = [];

    // Т.к. события берутся не из репозитория, а из Dictionary
    // на всякий случай обращение с ним делаем в рамках lock
    // Альтернативный вариант - использовать ConcurrentDictionary
    private object _lock = new object();

    public Task<IReadOnlyList<Event>> GetAllEventsAsync()
    {
        lock (_lock)
        {
            var result = _events.Values.ToList();

            return Task.FromResult((IReadOnlyList<Event>)result);
        }
    }

    public Task<Event?> FindEventByIdAsync(int id)
    {
        lock (_lock)
        {
            if (_events.TryGetValue(id, out Event? value))
            {
                return Task.FromResult<Event?>(value);
            }
            else
            {
                return Task.FromResult<Event?>(null);
            }
        }
    }

    public Task AddEventAsync(Event newEvent)
    {
        lock (_lock)
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
        }

        return Task.CompletedTask;
    }

    public Task UpdateEventAsync(Event eventForUpdate)
    {
        lock (_lock)
        {
            if (_events.ContainsKey(eventForUpdate.Id) is false)
                throw new KeyNotFoundException($"Event with id = {eventForUpdate.Id} is absent");

            _events[eventForUpdate.Id] = eventForUpdate;
        }

        return Task.CompletedTask;
    }

    public Task RemoveEventAsync(int id)
    {
        lock (_lock)
        {
            if (_events.ContainsKey(id) is false)
                throw new KeyNotFoundException($"Event with id = {id} is absent");

            _events.Remove(id);
        }
        return Task.CompletedTask;
    }
}
