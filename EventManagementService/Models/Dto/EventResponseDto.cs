namespace EventManagementService.Models.Dto;

public sealed class EventResponseDto
{
    public required string Title { get; init; }
    public required Guid Id { get; init; }
    public required string Description { get; init; } = string.Empty;
    public required int TotalSeats { get; init; }
    public required int AvailableSeats { get; init; }
    public required DateTime StartAt { get; init; }
    public required DateTime EndAt { get; init; }
}
