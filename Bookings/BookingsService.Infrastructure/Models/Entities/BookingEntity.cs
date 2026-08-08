using Bookings.Domain.Models;

namespace Bookings.Infrastructure.Models.Entities;

public sealed record BookingEntity(
    Guid Id,
    Guid EventId,
    Guid UserId,
    BookingStatus Status,
    DateTime CreatedAt,
    DateTime? ProcessedAt = null);