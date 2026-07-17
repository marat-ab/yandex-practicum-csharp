namespace EventManagementService.Domain.Exceptions;

public sealed class BookingAlreadyCancelledException : Exception
{
    public Guid BookingId { get; init; }

    public BookingAlreadyCancelledException()
        :base(message: "Unknown booking already cancelled error (without booking id)")
    {
    }

    public BookingAlreadyCancelledException(Guid bookingId, string message)
        : base(message: message)
    {
        BookingId = bookingId;
    }
}