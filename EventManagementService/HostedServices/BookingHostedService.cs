using EventManagementService.Middlewares;
using EventManagementService.Models;
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
        while (stoppingToken.IsCancellationRequested is false)
        {
            // Try/catch для перехвата исключений, которые могут быть выброшены при выгрузке всех бронирований
            try
            {
                // Получение списка бронирований в статусе Pending
                var pendingBooking = await _bookingRepository.SelectAllBookingByStatusAsync(BookingStatus.Pending, stoppingToken);

                // Имитация обращений к внешней системе
                var tasks = pendingBooking
                    .Select(_ => Task.Delay(TimeSpan.FromSeconds(2), stoppingToken))
                    .ToList();

                await Task.WhenAll(tasks);

                // Перевод брони в статус Confirmed и установка ProcessedAt
                foreach (var booking in pendingBooking)
                {
                    var updatedBooking = booking with
                    {
                        Status = BookingStatus.Confirmed,
                        ProcessedAt = DateTime.UtcNow,
                    };
                    
                    try
                    {
                        await _bookingRepository.UpdateBookingAsync(booking.Id, updatedBooking, stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(exception: ex, message: ex.Message);
                    }                    
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(exception: ex, message: ex.Message);
            }

            await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
        }
    }
}
