using Bookings.Domain.Models;
using Bookings.Infrastructure.DataAccess;
using Bookings.Infrastructure.Repositories;
using BookingsService.Application.Repositories;
using BookingsService.Application.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BookingsService.Tests.BookingServices;

public partial class BookingServiceTests : IAsyncLifetime
{
    private readonly ServiceProvider _serviceProvider;

    public BookingServiceTests()
    {
        var dbName = Guid.NewGuid().ToString();

        var services = new ServiceCollection();

        services.AddDbContext<AppDbContext>(options =>
            options.UseInMemoryDatabase(dbName));

        services.AddScoped<IBookingRepository, BookingRepository>();

        services.AddScoped<IBookingService, BookingService>();

        services.Configure<SystemSettings>(options =>
        {
            options.UserBookingLimit = 10;
        });

        _serviceProvider = services.BuildServiceProvider();
    }


    // IAsyncLifetime
    public async Task InitializeAsync()
    {

    }

    public async Task DisposeAsync()
    {

    }
}
