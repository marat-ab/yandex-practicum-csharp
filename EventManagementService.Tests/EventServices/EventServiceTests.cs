using EventManagementService.DataAccess;
using EventManagementService.Models;
using EventManagementService.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EventManagementService.Tests.EventServices;

public partial class EventServiceTests : IAsyncLifetime
{
    private static readonly List<Event> _events = [
            new Event(id: Guid.NewGuid(),
                title: "event 1",
                description: "Description of event 1",
                totalSeats: 1,
                startAt: new DateTime(2026, 01, 01),
                endAt: new DateTime(2026, 01, 03)),
            new Event(id: Guid.NewGuid(),
                title: "event 2",
                description: "Description of event 2",
                totalSeats: 1,
                startAt: new DateTime(2026, 02, 04),
                endAt: new DateTime(2026, 02, 05)),
            new Event(id: Guid.NewGuid(),
                title: "event 3",
                description: "Description of event 3",
                totalSeats: 1,
                startAt: new DateTime(2026, 03, 07),
                endAt: new DateTime(2026, 03, 10))];


    private readonly ServiceProvider _serviceProvider;

    public EventServiceTests()
    {
        var dbName = Guid.NewGuid().ToString();

        var services = new ServiceCollection();

        services.AddDbContext<AppDbContext>(options =>
            options.UseInMemoryDatabase(dbName));

        services.AddScoped<IEventService, EventService>();

        _serviceProvider = services.BuildServiceProvider();
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
        using var scope = _serviceProvider.CreateScope();
        var eventService = scope.ServiceProvider.GetRequiredService<IEventService>();

        for (int i = 0; i < _events.Count; i++)
        {
            var addedEvent = await eventService.AddEventAsync(_events[i]);
            _events[i] = addedEvent;
        }
    }

    public async Task DisposeAsync()
    {
        
    }
}
