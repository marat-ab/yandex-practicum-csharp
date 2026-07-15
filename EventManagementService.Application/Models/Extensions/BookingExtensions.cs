using EventManagementService.Application.Models.Dto;
using EventManagementService.Domain.Models;

namespace EventManagementService.Application.Models.Extensions;

public static class BookingExtensions
{
    public static BookingResponseDto ToBookingResponseDto(this Booking bookingData)
    {
        var result = new BookingResponseDto()
        {
            Id = bookingData.Id,
            EventId = bookingData.EventId,
            Status = bookingData.Status,
            CreatedAt = bookingData.CreatedAt,
            ProcessedAt = bookingData.ProcessedAt,
        };

        return result;
    }
}
