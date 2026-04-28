using EventManagementService.Models.Dto;
using Microsoft.AspNetCore.Mvc.Diagnostics;

namespace EventManagementService.Models.Extensions;

public static class EventExtensions
{
    public static EventResponseDto ToEventResponseDto(this Event eventData)
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

    public static Event ToEvent(this EventRequestDto eventRequest, Guid id)
    {
        if (eventRequest.StartAt == null || eventRequest.EndAt == null)
            throw new ArgumentException("StartAt and EndAt can't be null!");

        var result = new Event(
            Id: id,
            Title: eventRequest.Title,
            Description: eventRequest.Description,
            StartAt: eventRequest.StartAt.Value,
            EndAt: eventRequest.EndAt.Value);

        return result;
    }
}
