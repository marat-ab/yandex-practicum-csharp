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
    private readonly int _blankEventId = -1;

    private readonly IEventService _eventService;

    public EventsController(IEventService eventService)
    {
        _eventService = eventService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<EventResponseDto>>> GetAllEvents()
    {
        var result = (await _eventService.GetAllEventsAsync())
            .Select(x => x.ToEventResponse())
            .ToList();

        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<EventResponseDto>> GetEventById(int id)
    {
        var eventItem = await _eventService.GetEventByIdAsync(id);

        return Ok(eventItem.ToEventResponse());
    }

    [HttpPost]
    public async Task<ActionResult<EventResponseDto>> AddEvent([FromBody] EventRequestDto eventRequest)
    {
        var eventItem = eventRequest.ToEvent(_blankEventId);

        var eventItemWithId = await _eventService.AddEventAsync(eventItem);

        var result = eventItemWithId.ToEventResponse();

        return CreatedAtAction(nameof(AddEvent), result);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateEvent(int id, [FromBody] EventRequestDto eventForUpdate)
    {
        var eventData = eventForUpdate.ToEvent(id);
        await _eventService.UpdateEventAsync(eventData);

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteEvent(int id)
    {
        await _eventService.RemoveEventAsync(id);

        return NoContent();
    }

}
