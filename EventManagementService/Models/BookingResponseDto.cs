namespace EventManagementService.Models;

public sealed class BookingResponseDto
{
    public required Guid Id { get; init; }
    public required int EventId { get; init; }
    public required BookingStatus Status { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? ProcessedAt { get; init; } = null;
}
