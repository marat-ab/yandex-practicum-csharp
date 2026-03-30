using Microsoft.AspNetCore.Mvc.Diagnostics;

namespace EventManagementServices.Models.Extensions;

public static class EventExtensions
{
    public static EventResponse ToEventResponse(this Event eventData)
    {
        var result = new EventResponse()
        {
            Id = eventData.Id,
            Title = eventData.Title,
            Description = eventData.Description,
            StartAt = eventData.StartAt,
            EndAt = eventData.EndAt,
        };

        return result;
    }

    public static Event ToEvent(this EventRequest eventRequest, int id)
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
