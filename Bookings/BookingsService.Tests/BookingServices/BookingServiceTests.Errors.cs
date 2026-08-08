using Bookings.Domain.Exceptions;
using Bookings.Domain.Models;
using Bookings.Domain.Models.Auth;
using BookingsService.Application.Repositories;
using BookingsService.Application.Services;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace BookingsService.Tests.BookingServices;

public partial class BookingServiceTests
{
    // Получение брони по несуществующему id
    [Fact]
    [Trait("Category", "Success")]
    public async Task GetBookingWithNotExistingId()
    {
        // Arrange
        var userId = Guid.NewGuid();

        using var scope = _serviceProvider.CreateScope();
        var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();

        var bookingId = Guid.NewGuid();

        // Act
        Func<Task> act = async () => await bookingService.GetBookingByIdAsync(bookingId, userId);

        // Assert
        await act.Should().ThrowAsync<BookingNotFoundException>()
           .WithMessage($"Can't get booking with id = {bookingId}. It is absent");
    }

    // При достижении лимита активных броней новая бронь не создаётся
    [Fact]
    [Trait("Category", "Success")]
    public async Task CreateBookingWithLimitForSingleUser()
    {
        // Arrange
        var userId = Guid.NewGuid();

        using var scope = _serviceProvider.CreateScope();
        var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();
        var systemSettings = scope.ServiceProvider.GetRequiredService<IOptions<SystemSettings>>().Value;

        var eventId = Guid.NewGuid();

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
        var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();
        var bookingRepository = scope.ServiceProvider.GetRequiredService<IBookingRepository>();

        var eventId = Guid.NewGuid();

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
        var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();
        var bookingRepository = scope.ServiceProvider.GetRequiredService<IBookingRepository>();

        var eventId = Guid.NewGuid();

        // Act
        var booking = await bookingService.CreateBookingAsync(eventId, userId);
        await bookingService.CancelBookingAsync(booking.Id, userId, userRole);
        Func<Task> act = async () => await bookingService.CancelBookingAsync(booking.Id, userId, userRole);

        // Assert
        await act.Should().ThrowAsync<BookingAlreadyCancelledException>()
           .WithMessage($"Booking with id = {booking.Id} already cancelled");
    }
}
