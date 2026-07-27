namespace EventManagementService.Domain.Exceptions;

public sealed class BookingUserOverflowException : Exception
{
    public Guid UserId { get; init; }

    public BookingUserOverflowException()
        :base(message: "Unknown booking user overflow error (without user id)")
    {
    }

    public BookingUserOverflowException(Guid userId, string message)
        : base(message: message)
    {
        UserId = userId;
    }
}