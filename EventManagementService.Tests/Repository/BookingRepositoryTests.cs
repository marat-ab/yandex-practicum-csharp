using EventManagementService.Models;
using EventManagementService.Models.Extensions;
using EventManagementService.Repository;
using EventManagementService.Services;
using Microsoft.Extensions.Logging;

namespace EventManagementService.Tests.Repository;

public partial class BookingRepositoryTests : IAsyncLifetime
{
    private readonly IBookingRepository _bookingRepository;

    private static readonly List<Booking> _booking = [
        new Booking(Id: Guid.NewGuid(), EventId: Guid.NewGuid(), 
            Status: BookingStatus.Pending, CreatedAt: new DateTime(2026, 01, 01)),
        new Booking(Id: Guid.NewGuid(), EventId: Guid.NewGuid(),
            Status: BookingStatus.Pending, CreatedAt: new DateTime(2026, 02, 01)),
        new Booking(Id: Guid.NewGuid(), EventId: Guid.NewGuid(),
            Status: BookingStatus.Confirmed, CreatedAt: new DateTime(2026, 03, 01)),
           ];

    public BookingRepositoryTests()
    {
        _bookingRepository = new BookingRepository();
    }
   
    // IAsyncLifetime
    public async Task InitializeAsync()
    {
        foreach(var booking in _booking)
        {
            await _bookingRepository.InsertBookingAsync(booking.ToBookingEntity());
        }
    }

    public async Task DisposeAsync()
    {
        
    }
}
