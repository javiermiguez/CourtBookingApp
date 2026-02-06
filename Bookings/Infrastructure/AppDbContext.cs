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

            // Set primary key
            entity.HasKey(b => b.Id);

            // Set decimal precision for price
            entity.OwnsOne(b => b.Price, price =>
            {
                price.Property(p => p.Amount)
                    .HasPrecision(18, 2);
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
