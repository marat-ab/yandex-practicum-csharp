using EventManagementService.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventManagementService.Infrastructure.DataAccess.Configurations;

public class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.ToTable("events");

        builder.HasKey(b => b.Id);
        builder.Property(x => x.Id)
           .ValueGeneratedNever();

        builder.Property(b => b.Title)
           .IsRequired();

        builder.Property(b => b.Description)
           .IsRequired();

        builder.Property(b => b.TotalSeats)
           .IsRequired();

        builder.Property(b => b.AvailableSeats)
           .IsRequired();

        builder.Property(b => b.StartAt)
           .IsRequired();

        builder.Property(b => b.EndAt)
           .IsRequired();

        builder.HasMany(x => x.Bookings)
            .WithOne(x => x.Event)
            .HasForeignKey(x => x.EventId);
    }
}