using Bookings.Domain.Models;
using Bookings.Domain.Models.Auth;
using BookingsService.Application.Repositories;
using BookingsService.Application.Services;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace BookingsService.Tests.BookingServices;

public partial class BookingServiceTests
{
    // Создание брони для существующего события
    [Fact]
    [Trait("Category", "Success")]
    public async Task CreateBookingForExistingEvent()
    {
        // Arrange
        using var scope = _serviceProvider.CreateScope();
        var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();

        var eventId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        
        var expectedBookingStatus = BookingStatus.Pending;

        // Act
        var booking = await bookingService.CreateBookingAsync(eventId, userId);

        // Assert
        booking.Status.Should().Be(expectedBookingStatus);
    }

    // Создание нескольких броней для одного события 
    [Fact]
    [Trait("Category", "Success")]
    public async Task CreateMultipleBookingForExistingEvent()
    {
        // Arrange
        using var scope = _serviceProvider.CreateScope();
        var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();

        var userId = Guid.NewGuid();

        var eventId = Guid.NewGuid();
        var countOfBookings = 10;

        // Act
        var ids = new HashSet<Guid>();
        for (int i = 0; i < countOfBookings; i++)
        {
            var booking = await bookingService.CreateBookingAsync(eventId, userId);
            ids.Add(booking.Id);
        }

        // Assert
        ids.Count.Should().Be(countOfBookings);
    }

    // Получение брони по Id
    [Fact]
    [Trait("Category", "Success")]
    public async Task GetBookingById()
    {
        // Arrange
        var userId = Guid.NewGuid();

        using var scope = _serviceProvider.CreateScope();
        var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();

        var eventId = Guid.NewGuid();

        // Act
        var booking = await bookingService.CreateBookingAsync(eventId, userId);
        var bookingFromService = await bookingService.GetBookingByIdAsync(booking.Id, userId);

        // Assert
        booking.Should().BeEquivalentTo(bookingFromService);
    }

    // Получение брони отражает изменение статуса
    [Theory]
    [Trait("Category", "Success")]
    [InlineData(BookingStatus.Confirmed)]
    [InlineData(BookingStatus.Rejected)]
    public async Task BookingStatusWasChanged(BookingStatus newBookingStatus)
    {
        // Arrange
        var userId = Guid.NewGuid();

        using var scope = _serviceProvider.CreateScope();
        var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();

        var eventId = Guid.NewGuid();
        var expectedStatus = newBookingStatus;

        // Act
        var booking = await bookingService.CreateBookingAsync(eventId, userId);
        var updatedBooking = newBookingStatus switch
        {
            BookingStatus.Confirmed => booking.Confirm(),
            BookingStatus.Rejected => booking.Reject(),
            _ => throw new ArgumentException($"Test not work with booking status: {newBookingStatus}")
        };

        var bookingFromService = await bookingService.GetBookingByIdAsync(booking.Id, userId);

        // Assert
        bookingFromService.Status.Should().Be(newBookingStatus);
        bookingFromService.ProcessedAt.Should().NotBeNull();
    }
       
    // Тест на уникальность Id при конкурентных запросах
    [Fact]
    [Trait("Category", "Success")]
    public async Task UnicIdForConcurrencyRequests()
    {
        // Arrange
        var userId = Guid.NewGuid();

        using var scope = _serviceProvider.CreateScope();
        var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();

        var countOfSeats = 10;
        var eventId = Guid.NewGuid();

        var countOfExpectedBookings = 10;

        var tasks = new List<Task>(countOfExpectedBookings);
        for (int i = 0; i < countOfExpectedBookings; i++)
        {
            tasks.Add(Task.Run(async () =>
            {
                using var scope = _serviceProvider.CreateScope();

                var bookingService = scope.ServiceProvider
                    .GetRequiredService<IBookingService>();

                await bookingService.CreateBookingAsync(eventId, userId);
            }));
        }

        // Act
        await Task.WhenAll(tasks);

        var bookings = await bookingService.GetAllBookingByStatusAsync(BookingStatus.Pending);
        var ids = bookings.Select(x => x.Id).ToHashSet();

        // Assert
        bookings.Count.Should().Be(countOfExpectedBookings);
        ids.Count.Should().Be(countOfExpectedBookings);
    }

    // Лимиты разных пользователей не влияют друг на друга
    [Fact]
    [Trait("Category", "Success")]
    public async Task CreateBookingWithLimitsForMultiUsers()
    {
        // Arrange
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();
        var countOfBookingForSingleUser = 10;

        using var scope = _serviceProvider.CreateScope();
        var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();
        var bookingRepository = scope.ServiceProvider.GetRequiredService<IBookingRepository>();

        var eventId = Guid.NewGuid();
        var countOfBookings = 30;

        // Act
        for (int i = 0; i < countOfBookingForSingleUser; i++)
            await bookingService.CreateBookingAsync(eventId, userId1);

        for (int i = 0; i < countOfBookingForSingleUser; i++)
            await bookingService.CreateBookingAsync(eventId, userId2);

        // Assert
        var countOfBookingsForUser1 = (await bookingRepository.SelectAllActiveBookingForUserAsync(userId1)).Count;
        var countOfBookingsForUser2 = (await bookingRepository.SelectAllActiveBookingForUserAsync(userId2)).Count;

        countOfBookingsForUser1.Should().Be(countOfBookingForSingleUser);
        countOfBookingsForUser2.Should().Be(countOfBookingForSingleUser);
    }

    // Владелец брони отменяет свою бронь
    [Fact]
    [Trait("Category", "Success")]
    public async Task CancelSelfBooking()
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
        var bookingFromStore = await bookingRepository.SelectBookingByIdAsync(booking.Id);

        // Assert
        bookingFromStore.Status.Should().Be(BookingStatus.Cancelled);
    }

    // Администратор может отменить чужую бронь
    [Fact]
    [Trait("Category", "Success")]
    public async Task CancelNotSelfBookingByAdmin()
    {
        // Arrange
        var userId = Guid.NewGuid();

        var adminId = Guid.NewGuid();
        var adminRole = Role.Admin;

        using var scope = _serviceProvider.CreateScope();
        var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();
        var bookingRepository = scope.ServiceProvider.GetRequiredService<IBookingRepository>();

        var eventId = Guid.NewGuid();

        // Act
        var booking = await bookingService.CreateBookingAsync(eventId, userId);
        await bookingService.CancelBookingAsync(booking.Id, adminId, adminRole);
        var bookingFromStore = await bookingRepository.SelectBookingByIdAsync(booking.Id);

        // Assert
        bookingFromStore.Status.Should().Be(BookingStatus.Cancelled);
    }

}
