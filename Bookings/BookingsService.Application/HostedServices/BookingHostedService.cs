using Bookings.Domain.Models;
using BookingsService.Application.Brokers;
using BookingsService.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BookingsService.Application.HostedServices;

public class BookingHostedService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IBookingProducer _bookingProducer;

    private readonly ILogger<BookingHostedService> _logger;

    private readonly TimeSpan _bookingCheckCycleDelay = TimeSpan.FromSeconds(1);
    private readonly TimeSpan _bookingProcessingEmulationTime = TimeSpan.FromSeconds(2);

    private readonly SemaphoreSlim _processingSemaphore = new(1, 1);

    public BookingHostedService(
        IServiceScopeFactory scopeFactory,
        IBookingProducer bookingProducer,
        ILogger<BookingHostedService> logger)
    {
        _scopeFactory = scopeFactory;
        _bookingProducer = bookingProducer;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (stoppingToken.IsCancellationRequested is false)
        {
            // Try/catch для перехвата исключений, которые могут быть выброшены при выгрузке всех бронирований
            try
            {
                var pendingBookings = new List<Booking>();

                using (var scope = _scopeFactory.CreateScope())
                {
                    var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();
                    
                    // Получение списка бронирований в статусе Pending
                    var tmpBookings = await bookingService.GetAllBookingByStatusAsync(BookingStatus.Pending, stoppingToken);

                    pendingBookings.AddRange(tmpBookings);
                }

                // Обработка броней
                var tasks = pendingBookings.Select(booking => ProcessBookingAsync(booking, stoppingToken));

                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                _logger.LogError(exception: ex, message: ex.Message);
            }

            await Task.Delay(_bookingCheckCycleDelay, stoppingToken);
        }
    }

    private async Task ProcessBookingAsync(Booking booking, CancellationToken stoppingToken)
    {
        using var scope = _scopeFactory.CreateScope();

        var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();
        
        await Task.Delay(_bookingProcessingEmulationTime, stoppingToken);

        try
        {
            await _processingSemaphore.WaitAsync(stoppingToken);
                        
            var confirmedBooking = booking.Confirm();

            await bookingService.UpdateBookingAsync(booking.Id, confirmedBooking, stoppingToken);

            await _bookingProducer.BookingConfirmedAsync(confirmedBooking.EventId);

            _logger.LogInformation($"Booking confirmed with id: {confirmedBooking.EventId}");
        }
        catch (OperationCanceledException)
        {

        }
        catch (Exception ex)
        {
            // Отклонить бронь
            var rejectedBooking = booking.Reject();

            await bookingService.UpdateBookingAsync(booking.Id, rejectedBooking, stoppingToken);
                        
            _logger.LogError(exception: ex, message: ex.Message);
        }
        finally
        {
            _processingSemaphore.Release();
        }
    }
}
