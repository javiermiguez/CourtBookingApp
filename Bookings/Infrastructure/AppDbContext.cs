using Bookings.Domain;
using Microsoft.EntityFrameworkCore;

namespace Bookings.Infrastructure;

public class AppDbContext : DbContext
{
    public DbSet<Booking> Bookings => Set<Booking>();

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Booking>(entity =>
        {
            // NOTE: We'll only configure critical properties,
            // otherwise we can rely on EF Core conventions.

            entity.HasKey(b => b.Id); // Set primary key

            entity.OwnsOne(b => b.Configuration, configBuilder =>
            {
                configBuilder.Property(c => c.Modality)
                    .IsRequired();

                configBuilder.Property(c => c.GameType)
                    .IsRequired();
            });

            entity.OwnsOne(b => b.RequestPeriod, period =>
            {
                period.Property(p => p.Start)
                    .IsRequired();
                period.Property(p => p.End)
                    .IsRequired();
            });

            entity.OwnsOne(b => b.BookingPeriod, period =>
            {
                period.Property(p => p.Start)
                    .IsRequired();
                period.Property(p => p.End)
                    .IsRequired();
            });

            entity.OwnsOne(b => b.Price, price =>
            {
                price.Property(p => p.Amount)
                    .HasPrecision(18, 2) // Set decimal precision for price
                    .IsRequired();
                price.Property(p => p.Currency)
                    .IsRequired();
            });

            // Serialize Players as JSON for simplicity
            entity.Property(b => b.Players)
                .HasConversion(
                    v => System.Text.Json.JsonSerializer.Serialize(v),
                    v => System.Text.Json.JsonSerializer.Deserialize<List<Booking.Player>>(v)
                        ?? new List<Booking.Player>());
        });

        base.OnModelCreating(modelBuilder);
    }
}
