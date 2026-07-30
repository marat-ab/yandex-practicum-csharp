using Bookings.Domain.Models;
using BookingsService.Application.Brokers;
using BrokerLibrary.Kafka;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace BookingsService.Infrastructure.Brokers;

internal class BookingProducer : IBookingProducer, IDisposable
{
    private readonly ILogger<BookingProducer> _logger;
    private readonly KafkaSettings _kafkaSettings;

    private readonly IProducer<string, string> _producer;
    private bool _disposed;

    public BookingProducer(
        ILogger<BookingProducer> logger,
        IOptions<KafkaSettings> options)
    {
        _logger = logger;
        _kafkaSettings = options.Value;

        var config = new ProducerConfig
        {
            BootstrapServers = _kafkaSettings.BootstrapServers,
            Acks = Acks.All
        };

        _producer = new ProducerBuilder<string, string>(config).Build();
    }

    public async Task BookingConfirmedAsync(Guid eventId)
    {
        var bookingConfirmed = new BookingConfirmed(eventId);
                
        await _producer.ProduceAsync(KafkaTopics.BookingConfirmed, new Message<string, string>
        {
            Key = eventId.ToString(),
            Value = JsonSerializer.Serialize(bookingConfirmed)
        });
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;

        try
        {
            _producer.Flush(TimeSpan.FromSeconds(10));
        }
        catch (Exception ex)
        {
            _logger.LogError(exception: ex, message: "Some error occur when try flush producer");
        }
        finally
        {
            _producer.Dispose();
        }
    }
}
