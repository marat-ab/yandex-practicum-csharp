using EventManagementService.Models;
using EventManagementService.Repository;
using EventManagementService.Services;
using Microsoft.Extensions.Logging;

namespace EventManagementService.Tests.Repository;

public partial class BookingRepositoryTests : IAsyncLifetime
{
    private readonly IBookingRepository _bookingRepository;

    public BookingRepositoryTests()
    {
        _bookingRepository = new BookingRepository();
    }
   
    // IAsyncLifetime
    public async Task InitializeAsync()
    {
    }

    public async Task DisposeAsync()
    {
        
    }
}
