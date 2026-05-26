using EventManagementService.Middlewares;
using EventManagementService.Models;
using EventManagementService.Services;

namespace EventManagementService.HostedServices;

public class BookingHostedService : BackgroundService
{
    public readonly IBookingService _bookingService;
    public readonly IEventService _eventService;
    private readonly ILogger<BookingHostedService> _logger;

    private readonly SemaphoreSlim _processingSemaphore = new(1, 1);

    public BookingHostedService(
        IBookingService bookingService,
        IEventService eventService,
        ILogger<BookingHostedService> logger)
    {
        _bookingService = bookingService;
        _eventService = eventService;
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
                var pendingBookings = await _bookingService.GetAllBookingByStatusAsync(BookingStatus.Pending, stoppingToken);

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
        await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);

        try
        {
            await _processingSemaphore.WaitAsync();

            var eventTmp = await _eventService.FindEventByIdAsync(booking.EventId);

            // Событие не найдено
            if(eventTmp is null)
            {
                var rejectedBooking = booking.Reject();

                await _bookingService.UpdateBookingAsync(booking.Id, rejectedBooking, stoppingToken);
                
                _logger.LogWarning($"Event with id {booking.EventId} is absent.");

                return;
            }

            // Событие найдено
            var confirmedBooking = booking.Confirm();

            await _bookingService.UpdateBookingAsync(booking.Id, confirmedBooking, stoppingToken);

        }
        catch(OperationCanceledException)
        {

        }
        catch (Exception ex)
        {
            // Отклонить бронь
            var rejectedBooking = booking.Reject();

            await _bookingService.UpdateBookingAsync(booking.Id, rejectedBooking, stoppingToken);

            // Вернуть место
            var eventTmp = await _eventService.FindEventByIdAsync(booking.EventId);
            if (eventTmp is not null)
            {
                eventTmp.ReleaseSeats();
                await _eventService.UpdateEventAsync(eventTmp);
            }

            _logger.LogError(exception: ex, message: ex.Message);
        }
        finally
        {
            _processingSemaphore.Release();
        }


    }
}
