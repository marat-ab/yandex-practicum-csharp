namespace EventManagementService.Models;

public sealed class PaginatedResponseDto
{
    public required int TotalEventsCount { get; set; }
    public required List<EventResponseDto> Events { get; set; }
    public required int PageNumber { get; set; }
    public required int EventsCountOnPage { get; set; }
}
