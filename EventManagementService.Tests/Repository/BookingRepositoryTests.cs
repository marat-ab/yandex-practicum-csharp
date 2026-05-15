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
        new Booking(id: Guid.NewGuid(), eventId: Guid.NewGuid(), 
            status: BookingStatus.Pending, createdAt: new DateTime(2026, 01, 01)),
        new Booking(id: Guid.NewGuid(), eventId: Guid.NewGuid(),
            status: BookingStatus.Pending, createdAt: new DateTime(2026, 02, 01)),
        new Booking(id: Guid.NewGuid(), eventId: Guid.NewGuid(),
            status: BookingStatus.Confirmed, createdAt: new DateTime(2026, 03, 01)),
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
