namespace EventManagementService.Models.Extensions;

public static class PaginatedResultExtensions
{
    public static PaginatedResponseDto ToPaginatedResponseDto(this PaginatedResult data)
    {
        var eventsResponseDto = data.Events
            .Select(x => x.ToEventResponse())
            .ToList();

        var result = new PaginatedResponseDto()
        {
            TotalEventsCount = data.TotalEventsCount,
            Events = eventsResponseDto,
            PageNumber = data.PageNumber,
            EventsCountOnPage = data.EventsCountOnPage
        };

        return result;
    }
}
