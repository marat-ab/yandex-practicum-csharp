namespace EventManagementService.Models;

public sealed class Booking(
    Guid id,
    Guid eventId,
    BookingStatus status,
    DateTime createdAt,
    DateTime? processedAt = null)
{
    public Guid Id { get; set; } = id;
    public Guid EventId { get; set; } = eventId;
    public BookingStatus Status { get; set; } = status;
    public DateTime CreatedAt { get; set; } = createdAt;
    public DateTime? ProcessedAt { get; set; } = processedAt;

    public Booking Confirm()
    {
        Status = BookingStatus.Confirmed;
        ProcessedAt = DateTime.UtcNow;

        return this;
    }

    public Booking Reject()
    {
        Status = BookingStatus.Rejected;
        ProcessedAt = DateTime.UtcNow;

        return this;
    }
}