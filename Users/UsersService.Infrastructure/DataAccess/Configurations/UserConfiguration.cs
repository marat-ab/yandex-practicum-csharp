using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UsersService.Domain.Models.Auth;

namespace UsersService.Infrastructure.DataAccess.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(b => b.Id);
        builder.Property(x => x.Id)
           .ValueGeneratedNever();

        builder.Property(b => b.Login)
           .IsRequired();

        builder.Property(b => b.Role)
           .IsRequired()
           .HasConversion<string>();

        builder.Property(b => b.PasswordHash)
           .IsRequired();

        builder.HasIndex(u => u.Login)
           .IsUnique();
    }
}