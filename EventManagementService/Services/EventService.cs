using EventManagementService.Exceptions;
using EventManagementService.Models;

namespace EventManagementService.Services;

public class EventService : IEventService
{
    private readonly Dictionary<Guid, Event> _events = new();    

    // Т.к. события берутся не из репозитория, а из Dictionary
    // на всякий случай обращение с ним сделал в рамках lock'а
    // Альтернативный вариант - использовать ConcurrentDictionary
    private object _lock = new object();

    public EventService()
    {
        //var eventId1 = Guid.NewGuid();
        //var eventId2 = Guid.NewGuid();
        //var eventId3 = Guid.NewGuid();

        //_events = new()
        //{
        //    [eventId1] = new Event(
        //        Id: eventId1,
        //        Title: "a1",
        //        Description: "",
        //        StartAt: new DateTime(2026, 01, 01),
        //        EndAt: new DateTime(2026, 02, 01)),
        //    [eventId2] = new Event(
        //        Id: eventId2,
        //        Title: "b2",
        //        Description: "",
        //        StartAt: new DateTime(2026, 02, 02),
        //        EndAt: new DateTime(2026, 03, 01)),
        //    [eventId3] = new Event(
        //        Id: eventId3,
        //        Title: "c3",
        //        Description: "",
        //        StartAt: new DateTime(2026, 03, 02),
        //        EndAt: new DateTime(2026, 04, 01))
        //};
    }

    public Task<PaginatedResult> GetAllEventsAsync(
        string? title,
        DateTime? from,
        DateTime? to,
        int? pageNumber,
        int? pageSize)
    {
        if (to < from)
            throw new ArgumentException("DateTime to can't be less then from");

        if (pageNumber <= 0)
            throw new ArgumentException("pageNumber must be >= 1");

        if(pageSize <= 0)
            throw new ArgumentException("pageSize must be >= 1");

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

            var pgNumber = pageNumber ?? 1;
            var pgSize = pageSize ?? filtered.Count();

            var events = filtered
                .Skip((pgNumber - 1) * pgSize)
                .Take(pgSize)
                .ToList();

            var result = new PaginatedResult()
            {
                TotalEventsCount = filtered.Count(),
                Events = events,
                PageNumber = pgNumber,
                EventsCountOnPage = events.Count
            };

            return Task.FromResult(result);
        }
    }

    public Task<Event> GetEventByIdAsync(Guid id)
    {
        lock (_lock)
        {
            if (_events.TryGetValue(id, out Event? value))
            {
                return Task.FromResult<Event>(value);
            }
            else
            {
                throw new EventNotFoundException(id, 
                    $"Can't get event with id = {id}. It is absent");
            }
        }
    }

    public Task<Event?> FindEventByIdAsync(Guid id)
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
        if (string.IsNullOrWhiteSpace(newEvent.Title))
            throw new ArgumentException("Title can't be null, empty or white space");

        if (newEvent.StartAt == DateTime.MinValue)
            throw new ArgumentException("StartAt can't be min value");

        if (newEvent.EndAt == DateTime.MinValue)
            throw new ArgumentException("EndAt can't be min value");

        if(newEvent.EndAt < newEvent.StartAt)
            throw new ArgumentException("EndAt can't be less then StartAt");

        lock (_lock)
        {
            var id = newEvent.Id == Guid.Empty ? Guid.NewGuid() : newEvent.Id;
            var tmpEvent = new Event(
                Id: id,
                Title: newEvent.Title,
                Description: newEvent.Description,
                StartAt: newEvent.StartAt,
                EndAt: newEvent.EndAt);

            _events.Add(id, tmpEvent);

            return Task.FromResult(tmpEvent);
        }        
    }

    public Task UpdateEventAsync(Event eventForUpdate)
    {
        lock (_lock)
        {
            if (_events.ContainsKey(eventForUpdate.Id) is false)
                throw new EventNotFoundException(eventForUpdate.Id, 
                    $"Can't update event with id = {eventForUpdate.Id}. It is absent");

            _events[eventForUpdate.Id] = eventForUpdate;
        }

        return Task.CompletedTask;
    }

    public Task RemoveEventAsync(Guid id)
    {
        lock (_lock)
        {
            if (_events.ContainsKey(id) is false)
                throw new EventNotFoundException(id, 
                    $"Can't remove event with id = {id}. It is absent");

            _events.Remove(id);
        }
        return Task.CompletedTask;
    }
}
