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
            new Event(Id: Guid.NewGuid(),
                Title: "event 1",
                Description: "Description of event 1",
                StartAt: new DateTime(2026, 01, 01),
                EndAt: new DateTime(2026, 01, 03)),
            new Event(Id: Guid.NewGuid(),
                Title: "event 2",
                Description: "Description of event 2",
                StartAt: new DateTime(2026, 02, 04),
                EndAt: new DateTime(2026, 02, 05)),
            new Event(Id: Guid.NewGuid(),
                Title: "event 3",
                Description: "Description of event 3",
                StartAt: new DateTime(2026, 03, 07),
                EndAt: new DateTime(2026, 03, 10))];

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
