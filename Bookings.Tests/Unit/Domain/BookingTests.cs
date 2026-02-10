using Bookings.Common;
using Bookings.Domain;
using Bookings.Tests.Unit.Domain.TestFactories;

namespace Bookings.Tests.Unit.Domain;

public class BookingTests
{
    [Fact]
    public void Create_Booking_ShouldInitializeCorrectly()
    {
        var userId = Guid.NewGuid();
        var courtId = Guid.NewGuid();
        var booking = BookingTestFactory.CreateBooking(
            userId: userId, courtId: courtId);

        Assert.Equal(userId, booking.UserId);
        Assert.Equal(courtId, booking.CourtId);
        Assert.NotNull(booking.Price);
        Assert.Single(booking.Players);
    }

    [Fact]
    public void Create_Booking_WithPastStartTime_ShouldThrowException()
    {
        var pastPeriod = new Period(
            DateTime.UtcNow.AddHours(-2),
            DateTime.UtcNow.AddHours(-1));

        Assert.Throws<DomainException>(() =>
            BookingTestFactory.CreateBooking(period: pastPeriod));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void Create_Booking_WithInvalidPrice_ShouldThrowException(decimal invalidPrice)
    {
        Assert.Throws<DomainException>(() =>
            BookingTestFactory.CreateBooking(pricePerHour: invalidPrice));
    }

    [Fact]
    public void Create_Booking_ShouldHaveCorrectPrice()
    {
        var period = new Period(
            DateTime.UtcNow.AddHours(2),
            DateTime.UtcNow.AddHours(4));
        var pricePerHour = 30.0m;

        var booking = BookingTestFactory.CreateBooking(
            period: period, pricePerHour: pricePerHour);

        Assert.Equal((decimal)period.Duration.TotalHours * pricePerHour, booking.Price.Amount);
    }

    [Theory]
    [InlineData(BookingModality.Direct, BookingStatus.Pending)]
    [InlineData(BookingModality.Matchmaking, BookingStatus.WaitingForPlayers)]
    public void Create_Booking_ShouldHaveCorrectStatusBasedOnModality(
        BookingModality modality,
        BookingStatus expectedStatus)
    {
        var configuration = new BookingConfiguration(modality, GameType.Singles);
        var booking = BookingTestFactory.CreateBooking(configuration: configuration);

        Assert.Equal(expectedStatus, booking.Status);
    }

    [Fact]
    public void AddPlayer_ToMatchmakingBooking_ShouldWork()
    {
        var booking = BookingTestFactory.CreateBooking(
            configuration: new BookingConfiguration(BookingModality.Matchmaking, GameType.Doubles),
            rank: PlayerRank.Intermediate);

        var result = booking.AddPlayer(
            Guid.NewGuid(), PlayerRank.Intermediate);

        Assert.True(result.IsSuccess);
        Assert.Equal(Error.None, result.Error);
        Assert.Equal(2, booking.Players.Count);
    }

    [Fact]
    public void AddPlayer_ToDirectBooking_ShouldFail()
    {
        var booking = BookingTestFactory.CreateBooking(
            configuration: new BookingConfiguration(BookingModality.Direct, GameType.Doubles),
            rank: PlayerRank.Intermediate);

        var result = booking.AddPlayer(
            Guid.NewGuid(), PlayerRank.Intermediate);

        Assert.False(result.IsSuccess);
        Assert.Equal(BookingErrors.OnlyMatchmakingCanAddPlayers, result.Error);
    }

    [Theory]
    [InlineData(GameType.Singles, 2)]
    [InlineData(GameType.Doubles, 4)]
    public void Booking_ShouldRespectMaxPlayers(GameType matchType, int maxPlayers)
    {
        var booking = BookingTestFactory.CreateBooking(
            configuration: new BookingConfiguration(BookingModality.Matchmaking, matchType),
            rank: PlayerRank.Intermediate);

        for (int i = 1; i < maxPlayers; i++)
        {
            var result = booking.AddPlayer(
                Guid.NewGuid(), PlayerRank.Intermediate);
            Assert.True(result.IsSuccess);
        }

        var shouldFail = booking.AddPlayer(
            Guid.NewGuid(), PlayerRank.Intermediate);

        Assert.False(shouldFail.IsSuccess);
        Assert.Equal(BookingErrors.BookingNotWaiting, shouldFail.Error);
    }
}
