namespace EventManagementService.Exceptions;

public sealed class EventNotFoundException : Exception
{
    public int EventId { get; init; }

    public EventNotFoundException()
        :base(message: "Unknown event not found error (without event id)")
    {
    }

    public EventNotFoundException(int eventId, string message)
        : base(message: message)
    {
        EventId = eventId;
    }
}