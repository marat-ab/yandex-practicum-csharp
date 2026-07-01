using EventManagementService.Application.Models.Dto;
using EventManagementService.Domain.Models;
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
            TotalSeats = eventData.TotalSeats,
            AvailableSeats = eventData.AvailableSeats,
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
            id: id,
            title: eventRequest.Title,
            description: eventRequest.Description,
            totalSeats: eventRequest.TotalSeats,
            startAt: eventRequest.StartAt.Value,
            endAt: eventRequest.EndAt.Value);

        return result;
    }
}
