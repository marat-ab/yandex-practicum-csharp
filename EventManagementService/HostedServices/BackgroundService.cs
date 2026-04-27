using EventManagementService.Middlewares;
using EventManagementService.Repository;

namespace EventManagementService.HostedServices;

public class BookingHostedService : BackgroundService
{
    public readonly IBookingRepository _bookingRepository;
    private readonly ILogger<BookingHostedService> _logger;
    public BookingHostedService(
        IBookingRepository bookingRepository,
        ILogger<BookingHostedService> logger)
    {
        _bookingRepository = bookingRepository;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        
    }
}
