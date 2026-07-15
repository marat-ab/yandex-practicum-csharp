using EventManagementService.Domain.Models;
using EventManagementService.Infrastructure.Models.Entities;

namespace EventManagementService.Infrastructure.Models.Extensions;

public static class BookingRepositoryExtensions
{
    public static BookingEntity ToBookingEntity(this Booking booking)
    {
        var result = new BookingEntity(
            Id: booking.Id,
            EventId: booking.EventId,
            UserId: booking.UserId,
            Status: booking.Status,
            CreatedAt: booking.CreatedAt,
            ProcessedAt: booking.ProcessedAt);

        return result;
    }

    public static Booking ToBooking(this BookingEntity booking)
    {
        var result = new Booking(
            id: booking.Id,
            eventId: booking.EventId,
            userId: booking.UserId,
            status: booking.Status,
            createdAt: booking.CreatedAt,
            processedAt: booking.ProcessedAt);

        return result;
    }
}
