using EventManagementService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventManagementService.DataAccess.Configurations;

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
           .IsRequired();
        builder.Property(x => x.Status)
            .HasConversion<string>();

        builder.Property(b => b.CreatedAt)
           .IsRequired();

        builder.Property(b => b.ProcessedAt);

        builder.HasOne(b => b.Event)
            .WithMany(a => a.Bookings)
            .HasForeignKey(b => b.EventId);
    }
}