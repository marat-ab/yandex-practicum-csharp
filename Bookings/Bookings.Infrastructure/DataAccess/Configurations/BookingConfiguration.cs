using Bookings.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bookings.Infrastructure.DataAccess.Configurations;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.ToTable("bookings");

        builder.HasKey(b => b.Id);
        builder.Property(x => x.Id)
           .ValueGeneratedNever();

        builder.Property(b => b.EventId)
           .IsRequired();

        builder.Property(b => b.Status)
           .IsRequired()
           .HasConversion<string>();

        builder.Property(b => b.CreatedAt)
           .IsRequired();

        builder.Property(b => b.ProcessedAt);
    }
}