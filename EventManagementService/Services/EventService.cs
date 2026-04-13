using EventManagementService.Exceptions;
using EventManagementService.Models;

namespace EventManagementService.Services;

public class EventService : IEventService
{
    private int _lastId = 0;

    // Демо данные для тестов
    private readonly Dictionary<int, Event> _events = new()
    {
        [1] = new Event() { Id = 1, Title = "abc", StartAt = new DateTime(2026, 01, 01), EndAt = new DateTime(2026, 02, 01) },
        [2] = new Event() { Id = 2, Title = "abcdef", StartAt = new DateTime(2026, 02, 01), EndAt = new DateTime(2026, 03, 01) },
        [3] = new Event() { Id = 3, Title = "ghi", StartAt = new DateTime(2026, 03, 01), EndAt = new DateTime(2026, 04, 01) }
    };        

    // Т.к. события берутся не из репозитория, а из Dictionary
    // на всякий случай обращение с ним сделал в рамках lock'а
    // Альтернативный вариант - использовать ConcurrentDictionary
    private object _lock = new object();

    public Task<PaginatedResult> GetAllEventsAsync(
        string? title,
        DateTime? from,
        DateTime? to,
        int pageNumber = 1,
        int pageSize = 10)
    {
        lock (_lock)
        {
            var filtered = _events.Select(x => x.Value);

            if (title != null)
                filtered = filtered.Where(x => x.Title.ToLower().Contains(title.ToLower()));
            
            if (from != null)
                filtered = filtered.Where(x => x.StartAt >= from.Value);
            
            if (to != null)
                filtered = filtered.Where(x => x.EndAt <= to.Value);
            
            filtered = filtered.ToList();

            var events = filtered
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var result = new PaginatedResult()
            {
                TotalEventsCount = filtered.Count(),
                Events = events,
                PageNumber = pageNumber,
                EventsCountOnPage = events.Count()
            };

            return Task.FromResult(result);
        }
    }

    public Task<Event> GetEventByIdAsync(int id)
    {
        lock (_lock)
        {
            if (_events.TryGetValue(id, out Event? value))
            {
                return Task.FromResult<Event>(value);
            }
            else
            {
                throw new EventNotFoundException(id);
            }
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

    public Task<Event> AddEventAsync(Event newEvent)
    {
        lock (_lock)
        {
            _lastId++;
            var tmpEvent = new Event()
            {
                Id = _lastId,
                Title = newEvent.Title,
                Description = newEvent.Description,
                StartAt = newEvent.StartAt,
                EndAt = newEvent.EndAt,
            };

            _events.Add(_lastId, tmpEvent);

            return Task.FromResult(tmpEvent);
        }        
    }

    public Task UpdateEventAsync(Event eventForUpdate)
    {
        lock (_lock)
        {
            if (_events.ContainsKey(eventForUpdate.Id) is false)
                throw new EventNotFoundException(eventForUpdate.Id);

            _events[eventForUpdate.Id] = eventForUpdate;
        }

        return Task.CompletedTask;
    }

    public Task RemoveEventAsync(int id)
    {
        lock (_lock)
        {
            if (_events.ContainsKey(id) is false)
                throw new EventNotFoundException(id);

            _events.Remove(id);
        }
        return Task.CompletedTask;
    }
}
