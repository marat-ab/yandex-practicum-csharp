namespace EventsService.Domain.Exceptions;

public class EventAlreadyStartedException : Exception
{
    public Guid EventId { get; init; }

    public EventAlreadyStartedException()
        : base(message: "Event already started error (without event id)")
    {
    }

    public EventAlreadyStartedException(Guid eventId, string message)
        : base(message: message)
    {
        EventId = eventId;
    }
}