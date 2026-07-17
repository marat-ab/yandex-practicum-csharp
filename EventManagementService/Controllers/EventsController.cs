using EventManagementService.Application.Models.Dto;
using EventManagementService.Application.Models.Extensions;
using EventManagementService.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EventManagementService.Presentation.Controllers;

[Authorize]
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
    [AllowAnonymous]
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

    [AllowAnonymous]
    [HttpGet("{id:Guid}")]
    public async Task<ActionResult<EventResponseDto>> GetEventById(Guid id)
    {
        var eventItem = await _eventService.GetEventByIdAsync(id);

        return Ok(eventItem.ToEventResponseDto());
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<EventResponseDto>> AddEvent([FromBody] EventRequestDto eventRequest)
    {
        var eventItem = eventRequest.ToEvent(Guid.Empty);

        var eventItemWithId = await _eventService.AddEventAsync(eventItem);

        var result = eventItemWithId.ToEventResponseDto();

        return CreatedAtAction(
            nameof(GetEventById),
            new { id = result.Id },
            result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:Guid}")]
    public async Task<ActionResult> UpdateEvent(Guid id, [FromBody] EventRequestDto eventForUpdate)
    {
        var eventData = eventForUpdate.ToEvent(id);
        await _eventService.UpdateEventAsync(eventData);

        return NoContent();
    }

    [Authorize(Roles = "Admin")]
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
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null)        
            return BadRequest("User id not found");

        if (Guid.TryParse(userIdClaim.Value, out Guid userId))
        {
            var bookingItem = await _bookingService.CreateBookingAsync(eventId, userId);

            var url = $"/bookings/{bookingItem.Id}";
            Response.Headers.Location = url;

            var result = bookingItem.ToBookingResponseDto();

            return Accepted(result);
        }
        else
        {
            return BadRequest("Bad user id");
        }        
    }
}
