namespace EventManagementService.Models.Dto;

public sealed class PaginatedResultResponseDto
{
    public required int TotalEventsCount { get; set; }
    public required List<EventResponseDto> Events { get; set; }
    public required int PageNumber { get; set; }
    public required int EventsCountOnPage { get; set; }
}
