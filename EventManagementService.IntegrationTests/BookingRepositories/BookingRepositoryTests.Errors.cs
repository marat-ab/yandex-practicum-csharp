using EventManagementService.Domain.Exceptions;
using EventManagementService.Infrastructure.Repositories;
using FluentAssertions;

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
        var userId = Guid.NewGuid();
        await using var context = CreateContext();
        var bookingId = Guid.NewGuid();

        // Act
        var repository = new BookingRepository(context);
        Func<Task> act = async () => await repository.SelectBookingByIdAsync(bookingId, userId);

        // Assert
        await act.Should().ThrowAsync<BookingNotFoundException>()
           .WithMessage($"Can't get booking with id = {bookingId}. It is absent");
    }
}
