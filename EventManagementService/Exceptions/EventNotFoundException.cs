namespace EventManagementService.Exceptions;

public sealed class EventNotFoundException : Exception
{
    public int EventId { get; init; }

    public EventNotFoundException(int eventId)
        : base(message: $"Event with id = {eventId} is absent")
    {
        EventId = eventId;
    }
}