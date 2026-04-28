using EventManagementService.Models;
using EventManagementService.Repository;
using EventManagementService.Services;
using Microsoft.Extensions.Logging;

namespace EventManagementService.Tests.BookingServices;

public partial class BookingServiceTests : IAsyncLifetime
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IBookingService _bookingService;

    public BookingServiceTests()
    {
        _bookingRepository = new BookingRepository();
        _bookingService = new BookingService(_bookingRepository);
    }

   
    // IAsyncLifetime
    public async Task InitializeAsync()
    {
        
    }

    public async Task DisposeAsync()
    {
        
    }
}
