using EventManagementService.DataAccess;
using EventManagementService.Exceptions;
using EventManagementService.Models;
using EventManagementService.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EventManagementService.Services;

public class EventService : IEventService
{
    private readonly IEventRepository _eventRepository;

    public EventService(IEventRepository eventRepository)
    {
        _eventRepository = eventRepository;
    }

    public async Task<PaginatedResult> GetAllEventsAsync(
        string? title,
        DateTime? from,
        DateTime? to,
        int? pageNumber,
        int? pageSize, 
        CancellationToken ct = default)
    {
        if (to < from)
            throw new ArgumentException("DateTime to can't be less then from");

        if (pageNumber <= 0)
            throw new ArgumentException("pageNumber must be >= 1");

        if(pageSize <= 0)
            throw new ArgumentException("pageSize must be >= 1");

        var result = await _eventRepository.SelectAllEventsAsync(title: title, from: from, to: to,
            pageNumber: pageNumber, pageSize: pageSize, ct: ct);

        return result;
    }

    public async Task<Event> GetEventByIdAsync(Guid id, CancellationToken ct = default)
    {
        var result = await _eventRepository.SelectEventByIdAsync(id, ct);

        return result;
    }

    public async Task<Event?> FindEventByIdAsync(Guid id, CancellationToken ct = default)
    {
        try
        {
            var result = await _eventRepository.SelectEventByIdAsync(id, ct);

            if (result != null)
            {
                return result;
            }
            else
            {
                return null;
            }
        }
        catch (EventNotFoundException)
        {
            return null;
        }
    }

    public async Task<Event> AddEventAsync(Event newEvent, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(newEvent.Title))
            throw new ArgumentException("Title can't be null, empty or white space");

        if (newEvent.StartAt == DateTime.MinValue)
            throw new ArgumentException("StartAt can't be min value");

        if (newEvent.EndAt == DateTime.MinValue)
            throw new ArgumentException("EndAt can't be min value");

        if(newEvent.EndAt < newEvent.StartAt)
            throw new ArgumentException("EndAt can't be less then StartAt");

        if (newEvent.TotalSeats <= 0)
            throw new ArgumentException("TotalSeats can't be less or equal zero");

        var result = await _eventRepository.InsertEventAsync(newEvent, ct);

        return result;
    }

    public async Task UpdateEventAsync(Event eventValue, CancellationToken ct = default)
    {
        await _eventRepository.UpdateEventAsync(eventValue, ct);
    }

    public async Task RemoveEventAsync(Guid id, CancellationToken ct = default)
    {
        await _eventRepository.DeleteEventAsync(id, ct);
    }
}
