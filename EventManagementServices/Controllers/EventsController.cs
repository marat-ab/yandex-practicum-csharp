using EventManagementServices.Models;
using EventManagementServices.Models.Extensions;
using EventManagementServices.Services;
using Microsoft.AspNetCore.Mvc;

namespace EventManagementServices.Controllers;

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
    public async Task<ActionResult<IReadOnlyList<Event>>> GetAllEventsAsync()
    {
        var result = await _eventService.GetAllEventsAsync();

        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Event>> FindEventByIdAsync(int id)
    {
        var result = await _eventService.FindEventByIdAsync(id);

        if (result is not null)
            return Ok(result);
        else
            return NotFound();
    }

    [HttpPost]
    public async Task<ActionResult> AddEventAsync([FromBody] EventRequest eventRequest)
    {
        var eventData = eventRequest.ToEvent(_blankEventId);

        await _eventService.AddEventAsync(eventData);

        return Created();
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateEventAsync(int id, [FromBody] EventRequest eventForUpdate)
    {
        try
        {
            var eventData = eventForUpdate.ToEvent(id);
            await _eventService.UpdateEventAsync(eventData);
            return NoContent();
        }
        catch(KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteEventAsync(int id)
    {
        try
        {            
            await _eventService.RemoveEventAsync(id);
            return Ok();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

}
