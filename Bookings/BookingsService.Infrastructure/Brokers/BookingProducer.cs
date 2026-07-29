using Bookings.Domain.Models;
using BookingsService.Application.Brokers;
using BrokerLibrary.Kafka;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace BookingsService.Infrastructure.Brokers;

internal class BookingProducer : IBookingProducer
{
    private readonly KafkaSettings _kafkaSettings;

    public BookingProducer(IOptions<KafkaSettings> options)
    {
        _kafkaSettings = options.Value;
    }

    public async Task BookingConfirmedAsync(Guid eventId)
    {
        var bookingConfirmed = new BookingConfirmed(eventId);

        var config = new ProducerConfig
        {
            BootstrapServers = _kafkaSettings.BootstrapServers,
            Acks = Acks.All
        };

        using var producer = new ProducerBuilder<string, string>(config).Build();
        await producer.ProduceAsync(KafkaTopics.BookingConfirmed, new Message<string, string>
        {
            Key = eventId.ToString(),
            Value = JsonSerializer.Serialize(bookingConfirmed)
        });
    }
}
