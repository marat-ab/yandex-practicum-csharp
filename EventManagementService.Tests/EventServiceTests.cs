using EventManagementService.Models;
using EventManagementService.Services;

namespace EventManagementService.Tests;

public partial class EventServiceTests : IAsyncLifetime
{
    private readonly IEventService _eventService;
    private int _nextEventIdAfterInit;

    public EventServiceTests()
    {
        _eventService = new EventService();
    }

    // IAsyncLifetime
    public async Task InitializeAsync()
    {
        List<Event> events = [
            new Event(Id: 0,
                Title: "event 1",
                Description: "Description of event 1",
                StartAt: new DateTime(2026, 01, 01),
                EndAt: new DateTime(2026, 01, 03)),
            new Event(Id: 0,
                Title: "event 2",
                Description: "Description of event 2",
                StartAt: new DateTime(2026, 02, 04),
                EndAt: new DateTime(2026, 02, 05)),
            new Event(Id: 0,
                Title: "event 3",
                Description: "Description of event 3",
                StartAt: new DateTime(2026, 03, 07),
                EndAt: new DateTime(2026, 03, 10))];

        foreach(var e in events)
            await _eventService.AddEventAsync(e);

        _nextEventIdAfterInit = events.Count + 1;
    }

    public async Task DisposeAsync()
    {
        
    }
}
