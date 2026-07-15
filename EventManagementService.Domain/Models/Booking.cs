using EventManagementService.Domain.Models.Auth;

namespace EventManagementService.Domain.Models;

public sealed class Booking
{
    private Booking() { }

    public Booking(
        Guid id,
        Guid eventId,
        Guid userId,
        BookingStatus status,
        DateTime createdAt,
        DateTime? processedAt = null)
    {
        Id = id;
        EventId = eventId;
        UserId = userId;
        Status = status;
        CreatedAt = createdAt;
        ProcessedAt = processedAt;
    }

    public Guid Id { get; set; }
    public Guid EventId { get; set; }
    public Guid UserId { get; set; }
    public BookingStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }

    public Event Event { get; set; }
    public User User { get; set; }

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