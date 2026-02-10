using Bookings.Domain;
using Bookings.Infrastructure;
using Bookings.Tests.Unit.Domain.TestFactories;
using Microsoft.EntityFrameworkCore;

namespace Bookings.Tests.Unit.Infrastructure;

public class AppDbContextTests : IDisposable
{
    private readonly AppDbContext _context;

    public AppDbContextTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
    }

    [Fact]
    public async Task SaveChanges_PersistsBooking()
    {
        var booking = BookingTestFactory.CreateBooking();

        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        var saved = await _context.Bookings.FindAsync(booking.Id);
        Assert.NotNull(saved);
    }

    [Fact]
    public async Task Find_NonExistent_ReturnsNull()
    {
        var booking = await _context.Bookings.FindAsync(Guid.NewGuid());
        Assert.Null(booking);
    }

    [Fact]
    public async Task SaveChanges_PersistsPlayers()
    {
        var booking = BookingTestFactory.CreateBooking(
            configuration: new BookingConfiguration(BookingModality.Matchmaking, GameType.Doubles),
            rank: PlayerRank.Intermediate);
        booking.AddPlayer(Guid.NewGuid(), PlayerRank.Intermediate);
        booking.AddPlayer(Guid.NewGuid(), PlayerRank.Intermediate);

        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();
        await _context.Entry(booking).ReloadAsync();

        Assert.Equal(3, booking.Players.Count);
    }

    public void Dispose()
    {
        _context?.Dispose();
    }
}
