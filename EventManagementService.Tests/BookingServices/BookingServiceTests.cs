using EventManagementService.Application.Repositories;
using EventManagementService.Application.Services;
using EventManagementService.Domain.Models;
using EventManagementService.Infrastructure.DataAccess;
using EventManagementService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EventManagementService.Tests.BookingServices;

public partial class BookingServiceTests : IAsyncLifetime
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

    public BookingServiceTests()
    {
        var dbName = Guid.NewGuid().ToString();

        var services = new ServiceCollection();

        services.AddDbContext<AppDbContext>(options =>
            options.UseInMemoryDatabase(dbName));

        services.AddScoped<IEventRepository, EventRepository>();
        services.AddScoped<IBookingRepository, BookingRepository>();

        services.AddScoped<IEventService, EventService>();
        services.AddScoped<IBookingService, BookingService>();

        _serviceProvider = services.BuildServiceProvider();
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
