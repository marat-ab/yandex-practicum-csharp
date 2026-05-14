namespace EventManagementService.Models;

public sealed class Event
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int TotalSeats { get; set; }
    public int AvailableSeats { get; set; }
    public DateTime StartAt { get; set; }
    public DateTime EndAt { get; set; }

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
