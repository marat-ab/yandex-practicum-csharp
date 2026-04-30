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

        var expectedBookingStatus = BookingStatus.Pending;

        // Act
        var booking = await _bookingService.CreateBookingAsync(eventId);

        // Assert
        booking.Status.Should().Be(expectedBookingStatus);
    }

    // Создание нескольких броней для одного события 
    [Fact]
    [Trait("Category", "Success")]
    public async Task CreateMultipleBookingForExistingEvent()
    {
        // Arrange
        var eventId = _events[0].Id;
        var countOfBookings = 10;

        // Act
        var ids = new HashSet<Guid>();
        for (int i = 0; i < countOfBookings; i++)
        {
            var booking = await _bookingService.CreateBookingAsync(eventId);
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
        var updatedBooking = booking with
        {
            Status = newBookingStatus,
            ProcessedAt = DateTime.UtcNow,
        };
        await _bookingRepository.UpdateBookingAsync(booking.Id, updatedBooking.ToBookingEntity());

        var bookingFromService = await _bookingService.GetBookingByIdAsync(booking.Id);

        // Assert
        bookingFromService.Status.Should().Be(newBookingStatus);
    }
}
