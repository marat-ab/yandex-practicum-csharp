using EventsService.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventsService.Application.Services;

public interface ICacheService
{
    Task<Event> GetEventByIdAsync(Guid eventId);
    Task SetEventAsync(Event eventItem);
    Task<bool> DeleteEventByIdAsync(Guid eventId);
    Task<IReadOnlyList<Event>?> FindTopEventsAsync(int countInTop);
}
