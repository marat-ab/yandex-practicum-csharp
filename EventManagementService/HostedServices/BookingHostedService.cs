using EventManagementService.Middlewares;
using EventManagementService.Models;
using EventManagementService.Services;

namespace EventManagementService.HostedServices;

public class BookingHostedService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<BookingHostedService> _logger;

    private readonly SemaphoreSlim _processingSemaphore = new(1, 1);

    public BookingHostedService(
        IServiceScopeFactory scopeFactory,
        ILogger<BookingHostedService> logger)
    {
        _scopeFactory = scopeFactory;
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
                    var eventService = scope.ServiceProvider.GetRequiredService<IEventService>();

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

            await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
        }
    }

    private async Task ProcessBookingAsync(Booking booking, CancellationToken stoppingToken)
    {
        using var scope = _scopeFactory.CreateScope();
        
        var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();
        var eventService = scope.ServiceProvider.GetRequiredService<IEventService>();

        await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);

        try
        {
            await _processingSemaphore.WaitAsync(stoppingToken);

            var eventTmp = await eventService.FindEventByIdAsync(booking.EventId, stoppingToken);

            // Событие не найдено
            if(eventTmp is null)
            {
                var rejectedBooking = booking.Reject();

                await bookingService.UpdateBookingAsync(booking.Id, rejectedBooking, stoppingToken);
                
                _logger.LogWarning($"Event with id {booking.EventId} is absent.");

                return;
            }

            // Событие найдено
            var confirmedBooking = booking.Confirm();

            await bookingService.UpdateBookingAsync(booking.Id, confirmedBooking, stoppingToken);

        }
        catch(OperationCanceledException)
        {

        }
        catch (Exception ex)
        {
            // Отклонить бронь
            var rejectedBooking = booking.Reject();

            await bookingService.UpdateBookingAsync(booking.Id, rejectedBooking, stoppingToken);

            // Вернуть место
            var eventTmp = await eventService.FindEventByIdAsync(booking.EventId, stoppingToken);
            if (eventTmp is not null)
            {
                eventTmp.ReleaseSeats();
                await eventService.UpdateEventAsync(eventTmp, stoppingToken);
            }

            _logger.LogError(exception: ex, message: ex.Message);
        }
        finally
        {
            _processingSemaphore.Release();
        }
    }
}
