namespace EventManagementServices.Models;

public sealed class EventResponseDto
{
    public required int Id { get; init; }
    public required string Title { get; init; }
    public string Description { get; init; } = string.Empty;
    public required DateTime StartAt { get; init; }
    public required DateTime EndAt { get; init; }
}
