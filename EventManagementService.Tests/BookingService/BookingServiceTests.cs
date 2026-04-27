using EventManagementService.Models;
using EventManagementService.Services;
using Microsoft.Extensions.Logging;

namespace EventManagementService.Tests.BookingService;

public partial class BookingServiceTests : IAsyncLifetime
{
    private readonly IBookingService _bookingService;

    public BookingServiceTests()
    {
        _bookingService = new BookingService();
    }

   
    // IAsyncLifetime
    public async Task InitializeAsync()
    {
        
    }

    public async Task DisposeAsync()
    {
        
    }
}
