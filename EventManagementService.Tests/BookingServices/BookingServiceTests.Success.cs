using EventManagementService.Exceptions;
using EventManagementService.Models;
using EventManagementService.Models.Extensions;
using FluentAssertions;
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
        var eventId = _events[0].Id;

        var expectedAvailableSeats = 0;
        var expectedBookingStatus = BookingStatus.Pending;

        // Act
        var booking = await _bookingService.CreateBookingAsync(eventId);

        // Assert
        booking.Status.Should().Be(expectedBookingStatus);
        _events[0].AvailableSeats.Should().Be(expectedAvailableSeats);
    }

    // Создание нескольких броней для одного события 
    [Fact]
    [Trait("Category", "Success")]
    public async Task CreateMultipleBookingForExistingEvent()
    {
        // Arrange
        var eventId = _events[0].Id;        
        var countOfBookings = 10;
        _events[0].TotalSeats = countOfBookings;
        _events[0].AvailableSeats = countOfBookings;

        var expectedAvailableSeats = 0;

        // Act
        var ids = new HashSet<Guid>();
        for (int i = 0; i < countOfBookings; i++)
        {
            var booking = await _bookingService.CreateBookingAsync(eventId);
            ids.Add(booking.Id);
        }

        // Assert
        ids.Count.Should().Be(countOfBookings);
        _events[0].AvailableSeats.Should().Be(expectedAvailableSeats);
    }    

    // Получение брони по Id
    [Fact]
    [Trait("Category", "Success")]
    public async Task GetBookingById()
    {
        // Arrange
        var eventId = _events[0].Id;

        // Act
        var booking = await _bookingService.CreateBookingAsync(eventId);
        var bookingFromService = await _bookingService.GetBookingByIdAsync(booking.Id);

        // Assert
        booking.Should().Be(bookingFromService);
    }

    // Получение брони отражает изменение статуса
    [Theory]
    [Trait("Category", "Success")]
    [InlineData(BookingStatus.Confirmed)]
    [InlineData(BookingStatus.Rejected)]
    public async Task BookingStatusWasChanged(BookingStatus newBookingStatus)
    {
        // Arrange
        var eventId = _events[0].Id;
        var expectedStatus = newBookingStatus;
        // Act
        var booking = await _bookingService.CreateBookingAsync(eventId);
        var updatedBooking = newBookingStatus switch
        {
            BookingStatus.Confirmed => booking.Confirm(),
            BookingStatus.Rejected => booking.Reject(),
            _ => throw new ArgumentException($"Test not work with booking status: {newBookingStatus}")
        };

        await _bookingRepository.UpdateBookingAsync(booking.Id, updatedBooking.ToBookingEntity());

        var bookingFromService = await _bookingService.GetBookingByIdAsync(booking.Id);

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

        var countOfConcurrencyRequests = 20;
        var expectedCountWithExceptionNotAvailable = 15;
        var expectedAvailableSeats = 0;

        var tasks = new List<Task>(countOfConcurrencyRequests);
        for (int i = 0; i < countOfConcurrencyRequests; i++)
            tasks.Add(_bookingService.CreateBookingAsync(eventId));

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

        var bookings = await _bookingService.GetAllBookingByStatusAsync(BookingStatus.Pending);
        var eventValue = await _eventService.GetEventByIdAsync(eventId);

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
        var countOfSeats = 10;
        var eventId = _events[0].Id;
        _events[0].TotalSeats = countOfSeats;
        _events[0].AvailableSeats = countOfSeats;

        var countOfExpectedBookings = 10;

        var tasks = new List<Task>(countOfExpectedBookings);
        for (int i = 0; i < countOfExpectedBookings; i++)
            tasks.Add(_bookingService.CreateBookingAsync(eventId));

        // Act
        await Task.WhenAll(tasks);
                
        var bookings = await _bookingService.GetAllBookingByStatusAsync(BookingStatus.Pending);
        
        // Assert
        bookings.Count.Should().Be(countOfExpectedBookings);
    }
}
