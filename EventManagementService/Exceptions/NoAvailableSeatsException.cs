namespace EventManagementService.Exceptions;

public sealed class NoAvailableSeatsException : Exception
{
    public Guid EventId { get; init; }

    public NoAvailableSeatsException()
        : base(message: "No available seats error (without event id)")
    {
    }

    public NoAvailableSeatsException(Guid eventId, string message)
        : base(message: message)
    {
        EventId = eventId;
    }
}
