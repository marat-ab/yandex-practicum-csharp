namespace EventManagementService.Models;

public sealed class Event
{
    private Event() 
    {
        Title = string.Empty;
        Description = string.Empty;
    }

    public Event(
        Guid id, 
        string title, 
        string description, 
        int totalSeats, 
        DateTime startAt, 
        DateTime endAt)
    {
        Id = id;
        Title = title;
        Description = description;
        TotalSeats = totalSeats;
        AvailableSeats = totalSeats;
        StartAt = startAt;
        EndAt = endAt;
    }

    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int TotalSeats { get; set; }
    public int AvailableSeats { get; set; }
    public DateTime StartAt { get; set; }
    public DateTime EndAt { get; set; }

    public List<Booking> Bookings { get; set; }

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
