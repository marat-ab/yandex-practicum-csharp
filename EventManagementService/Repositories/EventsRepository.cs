using EventManagementService.DataAccess;
using EventManagementService.Exceptions;
using EventManagementService.Models;

using Microsoft.EntityFrameworkCore;

namespace EventManagementService.Repositories;

public class EventsRepository : IEventsRepository
{
    private readonly AppDbContext _dbc;

    private readonly SemaphoreSlim _eventSemaphore = new(1, 1);

    public EventsRepository(AppDbContext dbc)
    {
        _dbc = dbc;
    }

    // IEventsRepository
    public async Task<PaginatedResult> SelectAllEventsAsync(
        string? title = null,
        DateTime? from = null,
        DateTime? to = null,
        int? pageNumber = null,
        int? pageSize = null,
        CancellationToken ct = default)
    {
        try
        {
            await _eventSemaphore.WaitAsync(ct);

            var filtered = _dbc.Events.Select(x => x);

            if (title != null)
                filtered = filtered.Where(x => x.Title.Contains(title, StringComparison.CurrentCultureIgnoreCase));

            if (from != null)
                filtered = filtered.Where(x => x.StartAt >= from.Value);

            if (to != null)
                filtered = filtered.Where(x => x.EndAt <= to.Value);

            var pgNumber = pageNumber ?? 1;
            var pgSize = pageSize ?? filtered.Count();

            var events = await filtered
                .Skip((pgNumber - 1) * pgSize)
                .Take(pgSize)
                .ToListAsync(cancellationToken: ct);

            var result = new PaginatedResult()
            {
                TotalEventsCount = filtered.Count(),
                Events = events,
                PageNumber = pgNumber,
                EventsCountOnPage = events.Count
            };

            return result;
        }
        finally
        {
            _eventSemaphore.Release();
        }
    }

    public async Task<Event> SelectEventByIdAsync(Guid id, CancellationToken ct = default)
    {
        try
        {
            await _eventSemaphore.WaitAsync(ct);

            var result = _dbc.Events
                .Where(x => x.Id == id)
                .FirstOrDefault();

            if (result != null)
            {
                return result;
            }
            else
            {
                throw new EventNotFoundException(id,
                    $"Can't get event with id = {id}. It is absent");
            }
        }
        finally
        {
            _eventSemaphore.Release();
        }
    }

    public async Task<Event> InsertEventAsync(Event newEvent, CancellationToken ct = default)
    {
        try
        {
            await _eventSemaphore.WaitAsync(ct);

            var id = newEvent.Id == Guid.Empty ? Guid.NewGuid() : newEvent.Id;
            var tmpEvent = new Event(
                id: id,
                title: newEvent.Title,
                description: newEvent.Description,
                totalSeats: newEvent.TotalSeats,
                startAt: newEvent.StartAt,
                endAt: newEvent.EndAt);

            await _dbc.Events.AddAsync(tmpEvent, ct);
            await _dbc.SaveChangesAsync(ct);

            return tmpEvent;
        }
        finally
        {
            _eventSemaphore.Release();
        }
    }

    public async Task UpdateEventAsync(Event eventValue, CancellationToken ct = default)
    {
        try
        {
            await _eventSemaphore.WaitAsync(ct);

            var eventForUpdate = await _dbc.Events
                .Where(x => x.Id == eventValue.Id)
                .FirstOrDefaultAsync(ct);

            if (eventForUpdate is null)
            {
                throw new EventNotFoundException(eventValue.Id,
                    $"Can't update event with id = {eventValue.Id}. It is absent");
            }

            eventForUpdate.Title = eventValue.Title;
            eventForUpdate.Description = eventValue.Description;
            eventForUpdate.TotalSeats = eventValue.TotalSeats;
            eventForUpdate.AvailableSeats = eventValue.AvailableSeats;
            eventForUpdate.StartAt = eventValue.StartAt;
            eventForUpdate.EndAt = eventValue.EndAt;


            await _dbc.SaveChangesAsync(ct);
        }
        finally
        {
            _eventSemaphore.Release();
        }
    }

    public async Task DeleteEventAsync(Guid id, CancellationToken ct = default)
    {
        try
        {
            await _eventSemaphore.WaitAsync(ct);

            var eventForDelete = await _dbc.Events
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync(ct);

            if (eventForDelete is null)
            {
                throw new EventNotFoundException(id,
                    $"Can't remove event with id = {id}. It is absent");
            }

            _dbc.Events.Remove(eventForDelete);

            await _dbc.SaveChangesAsync(ct);
        }
        finally
        {
            _eventSemaphore.Release();
        }
    }
}
