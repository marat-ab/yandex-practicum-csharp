using EventManagementService.Domain.Models;

namespace EventManagementService.Application.Models.Entities;

public sealed record BookingEntity(
    Guid Id,
    Guid EventId,
    long UserId,
    BookingStatus Status,
    DateTime CreatedAt,
    DateTime? ProcessedAt = null);