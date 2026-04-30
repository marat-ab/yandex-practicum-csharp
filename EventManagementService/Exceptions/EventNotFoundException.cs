namespace EventManagementService.Exceptions;

public sealed class EventNotFoundException : Exception
{
    public Guid EventId { get; init; }

    public EventNotFoundException()
        :base(message: "Unknown event not found error (without event id)")
    {
    }

    public EventNotFoundException(Guid eventId, string message)
        : base(message: message)
    {
        EventId = eventId;
    }
}