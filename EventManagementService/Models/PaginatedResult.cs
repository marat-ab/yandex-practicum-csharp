namespace EventManagementService.Models;

public sealed class PaginatedResult
{
    public required int TotalEventsCount { get; set; }
    public required List<Event> Events { get; set; }
    public required int PageNumber { get; set; }
    public required int EventsCountOnPage { get; set; }
}
