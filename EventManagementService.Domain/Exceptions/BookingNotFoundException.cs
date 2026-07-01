namespace EventManagementService.Domain.Exceptions;

public sealed class BookingNotFoundException : Exception
{
    public Guid BookingId { get; init; }

    public BookingNotFoundException()
        :base(message: "Unknown booking not found error (without booking id)")
    {
    }

    public BookingNotFoundException(Guid bookingId, string message)
        : base(message: message)
    {
        BookingId = bookingId;
    }
}