using EventsService.Application.Repositories;
using EventsService.Application.Services;
using EventsService.Domain.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using static Confluent.Kafka.ConfigPropertyNames;

namespace EventsService.Infrastructure.Caches;

public class RedisCacheService : ICacheService
{
    private readonly IEventRepository _eventRepository;
    private readonly IDatabase _db;
    private readonly RedisSettings _redisSettings;

    private readonly ILogger<RedisCacheService> _logger;

    public RedisCacheService(
        IEventRepository eventRepository,
        IConnectionMultiplexer connection,
        ILogger<RedisCacheService> logger,
        IOptions<RedisSettings> options)
    {
        _eventRepository = eventRepository;
        _db = connection.GetDatabase();
        _logger = logger;

        _redisSettings = options.Value;        
    }

    public async Task<Event> GetEventByIdAsync(Guid eventId)
    {
        string key = $"event:{eventId}";

        try
        {
            RedisValue cached = await _db.StringGetAsync(key);

            if (cached.HasValue)
            {
                return JsonSerializer.Deserialize<Event>(cached.ToString());
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(exception: ex, message: "Can't get string from cache");
        }

        var eventItem = await _eventRepository.SelectEventByIdAsync(eventId);

        string json = JsonSerializer.Serialize(eventItem);
        try
        {
            await _db.StringSetAsync(key, json, TimeSpan.FromMinutes(_redisSettings.EventsTtlMinutes));
        }
        catch (Exception ex)
        {
            _logger.LogError(exception: ex, message: "Can't set string to cache");
        }

        return eventItem;
    }

    public async Task SetEventAsync(Event eventItem)
    {
        string key = $"event:{eventItem.Id}";

        string json = JsonSerializer.Serialize(eventItem);
        
        await _db.StringSetAsync(key, json, TimeSpan.FromMinutes(_redisSettings.EventsTtlMinutes));
    }

    public async Task<bool> DeleteEventByIdAsync(Guid eventId)
    {
        string key = $"event:{eventId}";

        var result = await _db.KeyDeleteAsync(key);

        return result;
    }

    public async Task<IReadOnlyList<Event>?> FindTopEventsAsync(int countInTop)
    {
        string key = "events:top10";

        try
        {
            RedisValue cached = await _db.StringGetAsync(key);

            if (cached.HasValue)
            {
                return JsonSerializer.Deserialize<List<Event>>(cached.ToString());
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(exception: ex, message: "Can't get string from cache");
        }

        var events = await _eventRepository.SelectAllEventsAsync();

        var topEvents = events.Events
            .Select(x => new { Event = x, PrcSales = (x.TotalSeats - x.AvailableSeats) / x.TotalSeats })
            .OrderByDescending(x => x.PrcSales)
            .Take(countInTop)
            .Select(x => x.Event)
            .ToList();

        string json = JsonSerializer.Serialize(topEvents);

        try
        {
            await _db.StringSetAsync(key, json, TimeSpan.FromMinutes(_redisSettings.TopEventsTtlMinutes));
        }
        catch (Exception ex)
        {
            _logger.LogError(exception: ex, message: "Can't set string to cache");
        }

        return topEvents;
    }
}
