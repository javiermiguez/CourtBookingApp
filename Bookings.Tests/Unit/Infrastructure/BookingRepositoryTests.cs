using Bookings.Domain;
using Bookings.Infrastructure;
using Bookings.Tests.Unit.Domain.TestFactories;
using Microsoft.EntityFrameworkCore;

namespace Bookings.Tests.Unit.Infrastructure;

public class BookingRepositoryTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly BookingRepository _repository;

    public BookingRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _repository = new BookingRepository(_context);
    }

    [Fact]
    public async Task GetById_Existing_ReturnsBooking()
    {
        var booking = BookingTestFactory.CreateBooking();
        await _repository.AddAsync(booking);

        var result = await _repository.GetByIdAsync(booking.Id);

        Assert.NotNull(result);
        Assert.Equal(booking.Id, result.Id);
    }

    [Fact]
    public async Task GetById_NonExistent_ReturnsNull()
    {
        var result = await _repository.GetByIdAsync(Guid.NewGuid());
        Assert.Null(result);
    }

    [Fact]
    public async Task Add_PersistsBooking()
    {
        var booking = BookingTestFactory.CreateBooking();

        await _repository.AddAsync(booking);
        var saved = await _context.Bookings.FindAsync(booking.Id);

        Assert.NotNull(saved);
        Assert.Equal(booking.Id, saved.Id);
    }

    [Fact]
    public async Task Add_DuplicateId_Throws()
    {
        var booking = BookingTestFactory.CreateBooking();
        await _repository.AddAsync(booking);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            _repository.AddAsync(booking));
    }

    [Fact]
    public async Task Update_ModifiesBooking()
    {
        var booking = BookingTestFactory.CreateBooking(
            configuration: new BookingConfiguration(BookingModality.Matchmaking, GameType.Doubles),
            rank: PlayerRank.Intermediate);
        await _repository.AddAsync(booking);

        booking.AddPlayer(Guid.NewGuid(), PlayerRank.Intermediate);
        await _repository.UpdateAsync(booking);

        var updated = await _repository.GetByIdAsync(booking.Id);
        Assert.Equal(2, updated!.Players.Count);
    }

    [Fact]
    public async Task FullFlow_AddGetUpdate_Works()
    {
        var booking = BookingTestFactory.CreateBooking(
            configuration: new BookingConfiguration(BookingModality.Matchmaking, GameType.Doubles),
            rank: PlayerRank.Intermediate);

        await _repository.AddAsync(booking);
        var retrieved = await _repository.GetByIdAsync(booking.Id);

        retrieved!.AddPlayer(Guid.NewGuid(), PlayerRank.Intermediate);
        await _repository.UpdateAsync(retrieved);

        var final = await _repository.GetByIdAsync(booking.Id);
        Assert.Equal(2, final!.Players.Count);
    }

    public void Dispose()
    {
        _context?.Dispose();
    }
}
