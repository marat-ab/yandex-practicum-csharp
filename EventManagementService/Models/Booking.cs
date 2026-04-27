namespace EventManagementService.Models;

public sealed record Booking(
    Guid Id,
    int EventId,
    BookingStatus Status,
    DateTime CreatedAt,
    DateTime? ProcessedAt = null);