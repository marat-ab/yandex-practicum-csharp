using EventManagementService.Application.Repositories;
using EventManagementService.Application.Services;
using EventManagementService.Domain.Exceptions;
using EventManagementService.Domain.Models;
using EventManagementService.Domain.Models.Auth;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace EventManagementService.Tests.BookingServices;

public partial class BookingServiceTests
{
    // Создание брони для не существующего события
    [Fact]
    [Trait("Category", "Success")]
    public async Task CreateBookingForNotExistingEvent()
    {
        // Arrange
        var userId = Guid.NewGuid();

        using var scope = _serviceProvider.CreateScope();
        var eventService = scope.ServiceProvider.GetRequiredService<IEventService>();
        var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();

        var eventId = Guid.NewGuid();

        // Act
        Func<Task> act = async () => await bookingService.CreateBookingAsync(eventId, userId);

        // Assert
        await act.Should().ThrowAsync<EventNotFoundException>()
           .WithMessage($"Absent event with id: {eventId}");
    }

    // Создание нескольких броней для одного события с исчерпанием количества свободных мест
    [Fact]
    [Trait("Category", "Success")]
    public async Task CreateMultipleBookingForExistingEventMoreThenAvailableSeats()
    {
        // Arrange
        var userId = Guid.NewGuid();

        using var scope = _serviceProvider.CreateScope();
        var eventService = scope.ServiceProvider.GetRequiredService<IEventService>();
        var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();

        var eventId = _events[0].Id;
        var countOfBookings = 10;
        _events[0].TotalSeats = countOfBookings - 1;
        _events[0].AvailableSeats = countOfBookings - 1;

        await eventService.UpdateEventAsync(_events[0]);

        // Act
        Func<Task>? act = null;
        for (int i = 0; i < countOfBookings; i++)
        {
            var eventFromDb = await eventService.GetEventByIdAsync(eventId);

            if (eventFromDb.AvailableSeats == 0)
            {
                act = async () => await bookingService.CreateBookingAsync(eventId, userId);
                break;
            }

            await bookingService.CreateBookingAsync(eventId, userId);
        }

        // Assert
        await act.Should().ThrowAsync<NoAvailableSeatsException>()
           .WithMessage($"No available seats for event with id {eventId}");
    }

    // Создание брони для удаленного события
    [Fact]
    [Trait("Category", "Success")]
    public async Task CreateBookingForDeletedEvent()
    {
        // Arrange
        var userId = Guid.NewGuid();

        using var scope = _serviceProvider.CreateScope();
        var eventService = scope.ServiceProvider.GetRequiredService<IEventService>();
        var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();

        var eventId = _events[0].Id;

        // Act
        await eventService.RemoveEventAsync(eventId);

        Func<Task> act = async () => await bookingService.CreateBookingAsync(eventId, userId);

        // Assert
        await act.Should().ThrowAsync<EventNotFoundException>()
           .WithMessage($"Absent event with id: {eventId}");
    }

    // Получение брони по несуществующему id
    [Fact]
    [Trait("Category", "Success")]
    public async Task GetBookingWithNotExistingId()
    {
        // Arrange
        var userId = Guid.NewGuid();

        using var scope = _serviceProvider.CreateScope();
        var eventService = scope.ServiceProvider.GetRequiredService<IEventService>();
        var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();

        var bookingId = Guid.NewGuid();

        // Act
        Func<Task> act = async () => await bookingService.GetBookingByIdAsync(bookingId, userId);

        // Assert
        await act.Should().ThrowAsync<BookingNotFoundException>()
           .WithMessage($"Can't get booking with id = {bookingId}. It is absent");
    }

    // Попытка забронировать прошедшее событие
    [Fact]
    [Trait("Category", "Success")]
    public async Task CreateBookingForPastEvent()
    {
        // Arrange
        var userId = Guid.NewGuid();

        using var scope = _serviceProvider.CreateScope();
        var eventService = scope.ServiceProvider.GetRequiredService<IEventService>();
        var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();

        var eventId = _events[0].Id;
        _events[0].StartAt = new DateTime(DateTime.Now.Year, 01, 01);
        await eventService.UpdateEventAsync(_events[0]);
        _events[0].StartAt = new DateTime(DateTime.Now.Year + 1, 01, 01);

        // Act
        Func<Task> act = async () => await bookingService.CreateBookingAsync(eventId, userId);

        // Assert
        await act.Should().ThrowAsync<EventAlreadyStartedException>()
           .WithMessage($"Event with id {eventId} is already started");
    }

    // При достижении лимита активных броней новая бронь не создаётся
    [Fact]
    [Trait("Category", "Success")]
    public async Task CreateBookingWithLimitForSingleUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        
        using var scope = _serviceProvider.CreateScope();
        var eventService = scope.ServiceProvider.GetRequiredService<IEventService>();
        var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();
        var systemSettings = scope.ServiceProvider.GetRequiredService<IOptions<SystemSettings>>().Value;

        var eventId = _events[0].Id;
        var countOfBookings = 20;
        _events[0].TotalSeats = countOfBookings - 1;
        _events[0].AvailableSeats = countOfBookings - 1;

        await eventService.UpdateEventAsync(_events[0]);

        // Act
        for (int i = 0; i < systemSettings.UserBookingLimit; i++)
            await bookingService.CreateBookingAsync(eventId, userId);

        Func<Task> act = async () => await bookingService.CreateBookingAsync(eventId, userId);

        // Assert
        await act.Should().ThrowAsync<BookingUserOverflowException>()
           .WithMessage($"Booking for user with id {userId} is overflowed. " +
           $"Limit: {systemSettings.UserBookingLimit}");
    }

    // Обычный пользователь не может отменить чужую бронь
    [Fact]
    [Trait("Category", "Success")]
    public async Task CancelNotSelfBookingByUser()
    {
        // Arrange
        var user1Id = Guid.NewGuid();

        var user2Id = Guid.NewGuid();
        var user2Role = Role.User;

        using var scope = _serviceProvider.CreateScope();
        var eventService = scope.ServiceProvider.GetRequiredService<IEventService>();
        var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();
        var bookingRepository = scope.ServiceProvider.GetRequiredService<IBookingRepository>();

        var eventId = _events[0].Id;

        // Act
        var booking = await bookingService.CreateBookingAsync(eventId, user1Id);
        Func<Task> act = async () => await bookingService.CancelBookingAsync(booking.Id, user2Id, user2Role);

        // Assert
        await act.Should().ThrowAsync<BookingAccessDeniedException>()
           .WithMessage($"User with id {user2Id} can't cancel booking " +
                    $"with id {booking.Id}. No access to this booking.");
    }


    // Двойная отмена одного и того же бронирования
    [Fact]
    [Trait("Category", "Success")]
    public async Task DoubleCancellation()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var userRole = Role.User;
        using var scope = _serviceProvider.CreateScope();
        var eventService = scope.ServiceProvider.GetRequiredService<IEventService>();
        var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();
        var bookingRepository = scope.ServiceProvider.GetRequiredService<IBookingRepository>();

        var eventId = _events[0].Id;

        // Act
        var booking = await bookingService.CreateBookingAsync(eventId, userId);
        await bookingService.CancelBookingAsync(booking.Id, userId, userRole);
        Func<Task> act = async () => await bookingService.CancelBookingAsync(booking.Id, userId, userRole);

        // Assert
        await act.Should().ThrowAsync<BookingAlreadyCancelledException>()
           .WithMessage($"Booking with id = {booking.Id} already cancelled");
    }
}
