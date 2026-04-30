using EventManagementService.Exceptions;
using EventManagementService.Models;
using EventManagementService.Models.Extensions;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventManagementService.Tests.Repository;

public partial class BookingRepositoryTests
{
    // Получение брони по несуществующему id из репозитория
    [Fact]
    [Trait("Category", "Success")]
    public async Task GetBookingWithNotExistingIdFromRepository()
    {
        // Arrange
        var bookingId = Guid.NewGuid();

        // Act
        Func<Task> act = async () => await _bookingRepository.SelectBookingByIdAsync(bookingId);

        // Assert
        await act.Should().ThrowAsync<BookingNotFoundException>()
           .WithMessage($"Can't get booking with id = {bookingId}. It is absent");
    }

    // Обновление брони по несуществующему id из репозитория
    [Fact]
    [Trait("Category", "Success")]
    public async Task UpdateBookingWithNotExistingIdFromRepository()
    {
        // Arrange
        var bookingId = Guid.NewGuid();
        var booking = new Booking(Id: bookingId, EventId: Guid.NewGuid(),
            Status: BookingStatus.Pending, CreatedAt: new DateTime(2026, 01, 01));

        // Act
        Func<Task> act = async () => await _bookingRepository.UpdateBookingAsync(bookingId, booking.ToBookingEntity());

        // Assert
        await act.Should().ThrowAsync<BookingNotFoundException>()
           .WithMessage($"Can't get booking with id = {bookingId}. It is absent");
    }
}
