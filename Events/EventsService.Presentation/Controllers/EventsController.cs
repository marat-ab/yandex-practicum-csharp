using EventsService.Application.Models.Dto;
using EventsService.Application.Services;
using EventsService.Application.Models.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EventsService.Presentation.Controllers;

[Authorize]
[ApiController]
[Route("/[controller]")]
public class EventsController : ControllerBase
{
    private readonly IEventService _eventService;

    public EventsController(IEventService eventService)
    {
        _eventService = eventService;
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
}
