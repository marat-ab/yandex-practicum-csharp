namespace EventManagementService.Models;

public sealed class Event(Guid id, string title, string description, int totalSeats, DateTime startAt, DateTime endAt)
{
    public Guid Id { get; set; } = id;
    public string Title { get; set; } = title;
    public string Description { get; set; } = description;
    public int TotalSeats { get; set; } = totalSeats;
    public int AvailableSeats { get; set; } = totalSeats;
    public DateTime StartAt { get; set; } = startAt;
    public DateTime EndAt { get; set; } = endAt;

    public bool TryReserveSeats(int count = 1)
    {
        if (AvailableSeats - count < 0)
            return false;

        AvailableSeats -= count;
        return true;
    }

    public void ReleaseSeats(int count = 1)
    {
        AvailableSeats += count;

        if(AvailableSeats > TotalSeats)
            AvailableSeats = TotalSeats;
    }
}
