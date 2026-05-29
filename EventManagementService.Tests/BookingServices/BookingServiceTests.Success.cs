using EventManagementService.Exceptions;
using EventManagementService.Models;
using EventManagementService.Models.Extensions;
using EventManagementService.Services;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventManagementService.Tests.BookingServices;

public partial class BookingServiceTests
{
    // Создание брони для существующего события
    [Fact]
    [Trait("Category", "Success")]
    public async Task CreateBookingForExistingEvent()
    {
        // Arrange
        using var scope = _serviceProvider.CreateScope();
        var eventService = scope.ServiceProvider.GetRequiredService<IEventService>();
        var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();

        var eventId = Guid.NewGuid();
        var newEvent = new Event(id: eventId,
               title: "event 4",
               description: "Description of event 4",
               totalSeats: 1,
               startAt: new DateTime(2026, 01, 01),
               endAt: new DateTime(2026, 01, 03));

        var expectedAvailableSeats = 0;
        var expectedBookingStatus = BookingStatus.Pending;

        await eventService.AddEventAsync(newEvent);

        // Act
        var booking = await bookingService.CreateBookingAsync(eventId);
        var eventAfterBooking = await eventService.GetEventByIdAsync(eventId);

        // Assert
        booking.Status.Should().Be(expectedBookingStatus);
        eventAfterBooking.AvailableSeats.Should().Be(expectedAvailableSeats);
    }

    // Создание нескольких броней для одного события 
    [Fact]
    [Trait("Category", "Success")]
    public async Task CreateMultipleBookingForExistingEvent()
    {
        // Arrange
        using var scope = _serviceProvider.CreateScope();
        var eventService = scope.ServiceProvider.GetRequiredService<IEventService>();
        var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();

        var eventId = _events[0].Id;        
        var countOfBookings = 10;
        _events[0].TotalSeats = countOfBookings;
        _events[0].AvailableSeats = countOfBookings;

        await eventService.UpdateEventAsync(_events[0]);

        var expectedAvailableSeats = 0;

        // Act
        var ids = new HashSet<Guid>();
        for (int i = 0; i < countOfBookings; i++)
        {
            var booking = await bookingService.CreateBookingAsync(eventId);
            ids.Add(booking.Id);
        }

        var eventFromDb = await eventService.GetEventByIdAsync(eventId);

        // Assert
        ids.Count.Should().Be(countOfBookings);
        eventFromDb.AvailableSeats.Should().Be(expectedAvailableSeats);
    }    

    // Получение брони по Id
    [Fact]
    [Trait("Category", "Success")]
    public async Task GetBookingById()
    {
        // Arrange
        using var scope = _serviceProvider.CreateScope();
        var eventService = scope.ServiceProvider.GetRequiredService<IEventService>();
        var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();

        var eventId = _events[0].Id;

        // Act
        var booking = await bookingService.CreateBookingAsync(eventId);
        var bookingFromService = await bookingService.GetBookingByIdAsync(booking.Id);

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
        using var scope = _serviceProvider.CreateScope();
        var eventService = scope.ServiceProvider.GetRequiredService<IEventService>();
        var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();

        var eventId = _events[0].Id;
        var expectedStatus = newBookingStatus;
        // Act
        var booking = await bookingService.CreateBookingAsync(eventId);
        var updatedBooking = newBookingStatus switch
        {
            BookingStatus.Confirmed => booking.Confirm(),
            BookingStatus.Rejected => booking.Reject(),
            _ => throw new ArgumentException($"Test not work with booking status: {newBookingStatus}")
        };

        var bookingFromService = await bookingService.GetBookingByIdAsync(booking.Id);

        // Assert
        bookingFromService.Status.Should().Be(newBookingStatus);
        bookingFromService.ProcessedAt.Should().NotBeNull();
    }

    // Защита от овербукинга
    [Fact]
    [Trait("Category", "Success")]
    public async Task OverbookingProtection()
    {
        // Arrange
        var countOfSeats = 5;
        var eventId = _events[0].Id;
        _events[0].TotalSeats = countOfSeats;
        _events[0].AvailableSeats = countOfSeats;

        using (var scopeArrange = _serviceProvider.CreateScope())
        {
            var eventServiceArrange = scopeArrange.ServiceProvider.GetRequiredService<IEventService>();
            await eventServiceArrange.UpdateEventAsync(_events[0]);
        }            

        var countOfConcurrencyRequests = 20;
        var expectedCountWithExceptionNotAvailable = 15;
        var expectedAvailableSeats = 0;

        var tasks = Enumerable.Range(0, countOfConcurrencyRequests)
            .Select(_ => Task.Run(async () =>
            {
                using var scope = _serviceProvider.CreateScope();
                var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();
                await bookingService.CreateBookingAsync(eventId);
            }))
            .ToList();        

        // Act
        var resultTask = Task.WhenAll(tasks);

        try
        {
            await resultTask;
        }
        catch { }

        var bookingsWithExceptionNotAvailable = 0;
        foreach(var task in tasks)
        {
            if (task.IsFaulted && task.Exception.InnerException is NoAvailableSeatsException)
                bookingsWithExceptionNotAvailable++;
        }

        using var scopeAct = _serviceProvider.CreateScope();
        var eventService = scopeAct.ServiceProvider.GetRequiredService<IEventService>();
        var bookingService = scopeAct.ServiceProvider.GetRequiredService<IBookingService>();

        var bookings = await bookingService.GetAllBookingByStatusAsync(BookingStatus.Pending);
        var eventValue = await eventService.GetEventByIdAsync(eventId);

        // Assert
        bookings.Count.Should().Be(countOfSeats);
        bookingsWithExceptionNotAvailable.Should().Be(expectedCountWithExceptionNotAvailable);
        eventValue.AvailableSeats.Should().Be(expectedAvailableSeats);
    }

    // Тест на уникальность Id при конкурентных запросах
    [Fact]
    [Trait("Category", "Success")]
    public async Task UnicIdForConcurrencyRequests()
    {
        // Arrange
        using var scope = _serviceProvider.CreateScope();
        var eventService = scope.ServiceProvider.GetRequiredService<IEventService>();
        var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();

        var countOfSeats = 10;
        var eventId = _events[0].Id;
        _events[0].TotalSeats = countOfSeats;
        _events[0].AvailableSeats = countOfSeats;

        await eventService.UpdateEventAsync(_events[0]);

        var countOfExpectedBookings = 10;

        var tasks = new List<Task>(countOfExpectedBookings);
        for (int i = 0; i < countOfExpectedBookings; i++)
        {
            tasks.Add(Task.Run(async () =>
            {
                using var scope = _serviceProvider.CreateScope();

                var bookingService = scope.ServiceProvider
                    .GetRequiredService<IBookingService>();

                await bookingService.CreateBookingAsync(eventId);
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
}
