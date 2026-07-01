using EventManagementService.Domain.Models;

namespace EventManagementService.Application.Models.Dto;

public sealed class BookingResponseDto
{
    public required Guid Id { get; init; }
    public required Guid EventId { get; init; }
    public required BookingStatus Status { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? ProcessedAt { get; init; } = null;
}
