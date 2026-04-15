using EventManagementService.Models;
using EventManagementService.Services;
using Microsoft.Extensions.Logging;

namespace EventManagementService.Tests;

public partial class EventServiceTests : IAsyncLifetime
{
    private readonly IEventService _eventService;

    private static readonly List<Event> _events = [
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

    public EventServiceTests()
    {
        _eventService = new EventService();
    }

    public static IEnumerable<object[]> GetEventsWithPagingData()
    {        
        yield return new object[] { 1, 10, _events };
        yield return new object[] { 1, 1, new List<Event>() { _events[0] } };
        yield return new object[] { 1, 2, new List<Event>() { _events[0], _events[1] } };
        yield return new object[] { 2, 1, new List<Event>() { _events[1] } };
        yield return new object[] { 3, 2, new List<Event>() { } };
    }

    public static IEnumerable<object[]> GetBadEventParams()
    {
        yield return new object[] { "", new DateTime(2026, 01, 01), new DateTime(2026, 01, 02), "Title can't be null, empty or white space" };
        yield return new object[] { "test", DateTime.MinValue, new DateTime(2026, 01, 02), "StartAt can't be min value" };
        yield return new object[] { "test", new DateTime(2026, 01, 01), DateTime.MinValue, "EndAt can't be min value" };
        yield return new object[] { "test", new DateTime(2026, 01, 02), new DateTime(2026, 01, 01), "EndAt can't be less then StartAt" };
    }


    // IAsyncLifetime
    public async Task InitializeAsync()
    {
        for (int i = 0; i < _events.Count; i++)
        {
            var addedEvent = await _eventService.AddEventAsync(_events[i]);
            _events[i] = addedEvent;
        }
    }

    public async Task DisposeAsync()
    {
        
    }
}
