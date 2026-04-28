namespace EventManagementService.Models;

public sealed record BookingEntity(
    Guid Id,
    Guid EventId,
    BookingStatus Status,
    DateTime CreatedAt,
    DateTime? ProcessedAt = null);