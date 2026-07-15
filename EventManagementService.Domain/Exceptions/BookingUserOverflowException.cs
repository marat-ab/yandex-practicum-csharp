namespace EventManagementService.Domain.Exceptions;

public sealed class BookingUserOverflowException : Exception
{
    public long UserId { get; init; }

    public BookingUserOverflowException()
        :base(message: "Unknown booking user overflow error (without user id)")
    {
    }

    public BookingUserOverflowException(long userId, string message)
        : base(message: message)
    {
        UserId = userId;
    }
}