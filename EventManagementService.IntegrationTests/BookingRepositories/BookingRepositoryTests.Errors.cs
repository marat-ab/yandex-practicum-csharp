using EventManagementService.Domain.Exceptions;
using EventManagementService.Models;
using EventManagementService.Repositories;
using EventManagementService.Services;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventManagementService.IntegrationTests.BookingRepositories;

public partial class BookingRepositoryTests
{
    // Получение брони по несуществующему id
    [Fact]
    [Trait("Category", "Success")]
    public async Task GetBookingWithNotExistingId()
    {
        await ResetDatabaseAsync();

        // Arrange
        await using var context = CreateContext();
        var bookingId = Guid.NewGuid();

        // Act
        var repository = new BookingRepository(context);
        Func<Task> act = async () => await repository.SelectBookingByIdAsync(bookingId);

        // Assert
        await act.Should().ThrowAsync<BookingNotFoundException>()
           .WithMessage($"Can't get booking with id = {bookingId}. It is absent");
    }
}
