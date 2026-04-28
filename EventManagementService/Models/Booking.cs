namespace EventManagementService.Models;

public sealed record Booking(
    Guid Id,
    Guid EventId,
    BookingStatus Status,
    DateTime CreatedAt,
    DateTime? ProcessedAt = null);