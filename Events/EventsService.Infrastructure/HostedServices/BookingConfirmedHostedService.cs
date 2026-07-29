using BrokerLibrary.Kafka;
using Confluent.Kafka;
using EventsService.Application.Services;
using EventsService.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace EventsService.Infrastructure.HostedServices;

internal class BookingConfirmedHostedService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<BookingConfirmedHostedService> _logger;

    private readonly KafkaSettings _kafkaSettings;

    private readonly TimeSpan _bookingConfirmedCheckCycleDelay = TimeSpan.FromMilliseconds(50);

    public BookingConfirmedHostedService(
        IServiceScopeFactory scopeFactory,
        ILogger<BookingConfirmedHostedService> logger,
        IOptions<KafkaSettings> options)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;

        _kafkaSettings = options.Value;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.Run(() => Consume(stoppingToken), stoppingToken);        
    }

    private async Task Consume(CancellationToken stoppingToken)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = _kafkaSettings.BootstrapServers,
            GroupId = _kafkaSettings.GroupId,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false
        };

        using var consumer = new ConsumerBuilder<string, string>(config).Build();
        consumer.Subscribe(KafkaTopics.BookingConfirmed);

        try
        {
            while (stoppingToken.IsCancellationRequested is false)
            {
                var consumeResult = consumer.Consume(stoppingToken);

                var bookingConfirmed = JsonSerializer.Deserialize<BookingConfirmed>(consumeResult.Message.Value);
                                
                using (var scope = _scopeFactory.CreateScope())
                {
                    var eventService = scope.ServiceProvider.GetRequiredService<IEventService>();

                    var eventTmp = await eventService.FindEventByIdAsync(bookingConfirmed.EventId, stoppingToken);

                    if (eventTmp is null)
                    {
                        _logger.LogWarning($"Absent event with id: {bookingConfirmed.EventId}");
                        continue;
                    }

                    var currentDt = DateTime.UtcNow;
                    if (currentDt >= eventTmp.StartAt)
                    {
                        _logger.LogWarning($"Event with id {bookingConfirmed.EventId} is already started");
                        continue;
                    }

                    var isReservOk = eventTmp.TryReserveSeats();
                    if (isReservOk is false)
                    {
                        _logger.LogWarning($"No available seats for event with id {bookingConfirmed.EventId}");
                        continue;
                    }

                    await eventService.UpdateEventAsync(eventTmp, stoppingToken);

                    consumer.Commit(consumeResult);

                    _logger.LogInformation($"Event reseved by booking with id: {bookingConfirmed.EventId}");
                }

                await Task.Delay(_bookingConfirmedCheckCycleDelay, stoppingToken);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Consumer stopped by cancellation.");
        }
        catch(Exception ex)
        {
            _logger.LogError(exception: ex, message: "Some error occur when booking confirmed processing.");
        }
        finally
        {
            consumer.Close();
        }
    }
}
