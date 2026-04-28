using EventManagementService.Exceptions;
using EventManagementService.Models;
using EventManagementService.Models.Extensions;
using EventManagementService.Services;
using Microsoft.AspNetCore.Mvc;

namespace EventManagementService.Controllers;

[ApiController]
[Route("/[controller]")]
public class EventsController : ControllerBase
{
    private readonly IEventService _eventService;
    private readonly IBookingService _bookingService;

    public EventsController(
        IEventService eventService, 
        IBookingService bookingService)
    {
        _eventService = eventService;
        _bookingService = bookingService;
    }

    // Events
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<PaginatedResultResponseDto>>> GetAllEvents(
        [FromQuery] string? title,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = (await _eventService.GetAllEventsAsync(title, from, to, page, pageSize))
            .ToPaginatedResponseDto();            

        return Ok(result);
    }

    [HttpGet("{id:Guid}")]
    public async Task<ActionResult<EventResponseDto>> GetEventById(Guid id)
    {
        var eventItem = await _eventService.GetEventByIdAsync(id);

        return Ok(eventItem.ToEventResponseDto());
    }

    [HttpPost]
    public async Task<ActionResult<EventResponseDto>> AddEvent([FromBody] EventRequestDto eventRequest)
    {
        var eventItem = eventRequest.ToEvent(Guid.Empty);

        var eventItemWithId = await _eventService.AddEventAsync(eventItem);

        var result = eventItemWithId.ToEventResponseDto();

        return CreatedAtAction(nameof(AddEvent), result);
    }

    [HttpPut("{id:Guid}")]
    public async Task<ActionResult> UpdateEvent(Guid id, [FromBody] EventRequestDto eventForUpdate)
    {
        var eventData = eventForUpdate.ToEvent(id);
        await _eventService.UpdateEventAsync(eventData);

        return NoContent();
    }

    [HttpDelete("{id:Guid}")]
    public async Task<ActionResult> DeleteEvent(Guid id)
    {
        await _eventService.RemoveEventAsync(id);

        return NoContent();
    }

    // Booking
    [HttpPost("{eventId:Guid}/book")]
    public async Task<ActionResult<BookingResponseDto>> BookingEvent(Guid eventId)
    {
        var bookingItem = await _bookingService.CreateBookingAsync(eventId);

        var url = $"/bookings/{bookingItem.Id}";
        Response.Headers.Location = url;

        var result = bookingItem.ToBookingResponseDto();

        return Accepted(result);
    }
}
