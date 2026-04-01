using Microsoft.AspNetCore.Mvc.Diagnostics;

namespace EventManagementService.Models.Extensions;

public static class EventExtensions
{
    public static EventResponseDto ToEventResponse(this Event eventData)
    {
        var result = new EventResponseDto()
        {
            Id = eventData.Id,
            Title = eventData.Title,
            Description = eventData.Description,
            StartAt = eventData.StartAt,
            EndAt = eventData.EndAt,
        };

        return result;
    }

    public static Event ToEvent(this EventRequestDto eventRequest, int id)
    {
        var result = new Event()
        {
            Id = id,
            Title = eventRequest.Title,
            Description = eventRequest.Description,
            StartAt = eventRequest.StartAt,
            EndAt = eventRequest.EndAt,
        };

        return result;
    }
}
