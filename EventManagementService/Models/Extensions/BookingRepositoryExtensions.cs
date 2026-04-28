using EventManagementService.Models.Entities;

namespace EventManagementService.Models.Extensions;

public static class BookingRepositoryExtensions
{
    public static BookingEntity ToBookingEntity(this Booking booking)
    {
        var result = new BookingEntity(
            Id: booking.Id,
            EventId: booking.EventId,
            Status: booking.Status,
            CreatedAt: booking.CreatedAt,
            ProcessedAt: booking.ProcessedAt);

        return result;
    }

    public static Booking ToBooking(this BookingEntity booking)
    {
        var result = new Booking(
            Id: booking.Id,
            EventId: booking.EventId,
            Status: booking.Status,
            CreatedAt: booking.CreatedAt,
            ProcessedAt: booking.ProcessedAt);

        return result;
    }
}
