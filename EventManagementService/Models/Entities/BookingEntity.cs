namespace EventManagementService.Models.Entities;

public sealed record BookingEntity(
    Guid Id,
    Guid EventId,
    BookingStatus Status,
    DateTime CreatedAt,
    DateTime? ProcessedAt = null);