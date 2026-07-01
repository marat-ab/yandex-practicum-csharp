using EventManagementService.Application.Services;
using EventManagementService.Domain.Exceptions;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace EventManagementService.Tests.BookingServices;

public partial class BookingServiceTests
{
    // Создание брони для не существующего события
    [Fact]
    [Trait("Category", "Success")]
    public async Task CreateBookingForNotExistingEvent()
    {
        // Arrange
        using var scope = _serviceProvider.CreateScope();
        var eventService = scope.ServiceProvider.GetRequiredService<IEventService>();
        var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();

        var eventId = Guid.NewGuid();

        // Act
        Func<Task> act = async () => await bookingService.CreateBookingAsync(eventId);

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
                act = async () => await bookingService.CreateBookingAsync(eventId);
                break;
            }

            await bookingService.CreateBookingAsync(eventId);
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
        using var scope = _serviceProvider.CreateScope();
        var eventService = scope.ServiceProvider.GetRequiredService<IEventService>();
        var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();

        var eventId = _events[0].Id;

        // Act
        await eventService.RemoveEventAsync(eventId);

        Func<Task> act = async () => await bookingService.CreateBookingAsync(eventId);

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
        using var scope = _serviceProvider.CreateScope();
        var eventService = scope.ServiceProvider.GetRequiredService<IEventService>();
        var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();

        var bookingId = Guid.NewGuid();

        // Act
        Func<Task> act = async () => await bookingService.GetBookingByIdAsync(bookingId);

        // Assert
        await act.Should().ThrowAsync<BookingNotFoundException>()
           .WithMessage($"Can't get booking with id = {bookingId}. It is absent");
    }
}
