using EventManagementService.Models;
using EventManagementService.Repository;
using EventManagementService.Services;
using Microsoft.Extensions.Logging;

namespace EventManagementService.Tests.BookingServices;

public partial class BookingServiceTests : IAsyncLifetime
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IEventService _eventService;
    private readonly IBookingService _bookingService;

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

    public BookingServiceTests()
    {
        _bookingRepository = new BookingRepository();
        _eventService = new EventService();
        _bookingService = new BookingService(_bookingRepository, _eventService);
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
