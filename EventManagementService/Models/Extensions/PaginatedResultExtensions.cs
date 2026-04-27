namespace EventManagementService.Models.Extensions;

public static class PaginatedResultExtensions
{
    public static PaginatedResultResponseDto ToPaginatedResponseDto(this PaginatedResult data)
    {
        var eventsResponseDto = data.Events
            .Select(x => x.ToEventResponse())
            .ToList();

        var result = new PaginatedResultResponseDto()
        {
            TotalEventsCount = data.TotalEventsCount,
            Events = eventsResponseDto,
            PageNumber = data.PageNumber,
            EventsCountOnPage = data.EventsCountOnPage
        };

        return result;
    }
}
