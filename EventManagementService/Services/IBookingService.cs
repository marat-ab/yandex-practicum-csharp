namespace EventManagementService.Services;

public interface IBookingService
{
    Task CreateBookingAsync(int eventId);

    Task GetBookingByIdAsync(Guid bookingId);
}
