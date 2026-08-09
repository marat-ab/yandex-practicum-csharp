using EventsService.Application.Repositories;
using EventsService.Application.Services;
using EventsService.Domain.Exceptions;
using EventsService.Domain.Models;
using EventsService.Infrastructure.Caches;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using StackExchange.Redis;
using System.Text.Json;

namespace EventsService.Tests.EventServices;

public partial class EventServiceTests
{
    // При попадании в кеш репозиторий не вызывается
    [Fact]
    [Trait("Category", "Success")]
    public async Task WhenCacheHitThenRepoNotUsed()
    {
        // Arrange
        var eventId = _events[0].Id;

        var mockEventRepository = new Mock<IEventRepository>();
        var mockCacheService = new Mock<ICacheService>();

        var eventService = new EventService(mockCacheService.Object, mockEventRepository.Object);

        mockCacheService.Setup(mock => mock.GetEventByIdAsync(eventId))
            .ReturnsAsync(_events[0]);

        // Act
        var eventItem = await eventService.GetEventByIdAsync(eventId);

        // Assert
        mockEventRepository.Verify(mock =>
            mock.SelectEventByIdAsync(eventId), Times.Never);
    }

    // При промахе данные берутся из репозитория и сохраняются в кеш
    [Fact]
    [Trait("Category", "Success")]
    public async Task WhenCacheMissThenRepoUsedAndStoreInCache()
    {
        // Arrange
        var eventId = _events[0].Id;
        var key = $"event:{eventId}";
        using var scope = _serviceProvider.CreateScope();
        var options = scope.ServiceProvider.GetRequiredService<IOptions<RedisSettings>>();

        var mockEventRepository = new Mock<IEventRepository>();
        var mockConnectionMultiplexer = new Mock<IConnectionMultiplexer>();
        var mockRedisDatabase = new Mock<IDatabase>();

        mockConnectionMultiplexer.Setup(mock => mock.GetDatabase())
           .Returns(mockRedisDatabase.Object);

        mockEventRepository.Setup(mock => mock.SelectEventByIdAsync(eventId))
            .ReturnsAsync(_events[0]);

        var cacheService = new RedisCacheService(mockEventRepository.Object, mockConnectionMultiplexer.Object,
            options);

        var eventService = new EventService(cacheService, mockEventRepository.Object);

        mockRedisDatabase.Setup(mock => mock.StringGetAsync(key))
            .ReturnsAsync(new RedisValue());

        string json = JsonSerializer.Serialize(_events[0]);

        mockRedisDatabase.Setup(mock => mock.StringSetAsync(key, json, It.IsAny<Expiration>()))
            .ReturnsAsync(true);

        // Act
        var eventItem = await eventService.GetEventByIdAsync(eventId);

        // Assert
        mockEventRepository.Verify(mock => mock.SelectEventByIdAsync(eventId), Times.Once);
        mockRedisDatabase.Verify(mock => mock.StringGetAsync(key), Times.Once);
        mockRedisDatabase.Verify(mock => mock.StringSetAsync(key, json, It.IsAny<Expiration>()), Times.Once);
    }

    // При мутирующих операциях кеш обновляется или инвалидируется в соответствии с выбранной стратегией.
    [Fact]
    [Trait("Category", "Success")]
    public async Task WhenEventUpdateThenCacheInvalidate()
    {
        // Arrange
        var eventId = _events[0].Id;

        var mockEventRepository = new Mock<IEventRepository>();
        var mockCacheService = new Mock<ICacheService>();

        mockEventRepository.Setup(mock => mock.UpdateEventAsync(It.IsAny<Event>()))
            .Returns(Task.CompletedTask);

        mockCacheService.Setup(mock => mock.DeleteEventByIdAsync(eventId))
            .ReturnsAsync(true);

        var eventService = new EventService(mockCacheService.Object, mockEventRepository.Object);

        // Act
        await eventService.UpdateEventAsync(_events[0]);

        // Assert
        mockEventRepository.Verify(mock =>
            mock.UpdateEventAsync(It.IsAny<Event>()), Times.Once);

        mockCacheService.Verify(mock =>
            mock.DeleteEventByIdAsync(eventId), Times.Once);
    }

    [Fact]
    [Trait("Category", "Success")]
    public async Task WhenEventDeleteThenCacheInvalidate()
    {
        // Arrange
        var eventId = _events[0].Id;

        var mockEventRepository = new Mock<IEventRepository>();
        var mockCacheService = new Mock<ICacheService>();

        mockEventRepository.Setup(mock => mock.DeleteEventAsync(eventId))
            .Returns(Task.CompletedTask);

        mockCacheService.Setup(mock => mock.DeleteEventByIdAsync(eventId))
            .ReturnsAsync(true);

        var eventService = new EventService(mockCacheService.Object, mockEventRepository.Object);

        // Act
        await eventService.RemoveEventAsync(_events[0].Id);

        // Assert
        mockEventRepository.Verify(mock => mock.DeleteEventAsync(eventId), Times.Once);

        mockCacheService.Verify(mock => mock.DeleteEventByIdAsync(eventId), Times.Once);
    }
}
