using Bookings.Common;
using Bookings.Domain;
using Bookings.Tests.Unit.Domain.TestFactories;
using Xunit;
using MatchType = Bookings.Domain.MatchType;

namespace Bookings.Tests.Unit.Domain;

public class BookingTests
{
    [Fact]
    public void Create_Booking_ShouldInitializeCorrectly()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var courtId = Guid.NewGuid();

        // Act
        var booking = BookingTestFactory.CreateTestBooking(
            userId: userId, courtId: courtId);

        // Assert
        Assert.Equal(userId, booking.UserId);
        Assert.Equal(courtId, booking.CourtId);
        Assert.NotNull(booking.Price);
        Assert.Single(booking.Players);
    }

    [Fact]
    public void Create_Booking_WithPastStartTime_ShouldThrowException()
    {
        // Arrange
        var pastPeriod = new Period(
            DateTime.UtcNow.AddHours(-2),
            DateTime.UtcNow.AddHours(-1));

        // Act & Assert
        Assert.Throws<DomainException>(() =>
            BookingTestFactory.CreateTestBooking(period: pastPeriod));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void Create_Booking_WithInvalidPrice_ShouldThrowException(decimal invalidPrice)
    {
        // Act & Assert
        Assert.Throws<DomainException>(() =>
            BookingTestFactory.CreateTestBooking(pricePerHour: invalidPrice));
    }

    [Fact]
    public void Create_Booking_ShouldHaveCorrectPrice()
    {
        // Arrange
        var period = new Period(
            DateTime.UtcNow.AddHours(2),
            DateTime.UtcNow.AddHours(4));

        var pricePerHour = 30.0m;

        // Act
        var booking = BookingTestFactory.CreateTestBooking(
            period: period, pricePerHour: pricePerHour);

        // Assert
        Assert.Equal((decimal)period.Duration().TotalHours * pricePerHour, booking.Price.Amount);
    }

    [Theory]
    [InlineData(BookingModality.Direct, BookingStatus.Pending)]
    [InlineData(BookingModality.Matchmaking, BookingStatus.WaitingForPlayers)]
    public void Create_Booking_ShouldHaveCorrectStatusBasedOnModality(
        BookingModality modality,
        BookingStatus expectedStatus)
    {
        // Arrange
        var configuration = new BookingConfiguration(modality, MatchType.Singles);

        // Act
        var booking = BookingTestFactory.CreateTestBooking(configuration: configuration);

        // Assert
        Assert.Equal(expectedStatus, booking.Status);
    }

    [Fact]
    public void AddPlayer_ToMatchmakingBooking_ShouldWork()
    {
        // Arrange
        var booking = BookingTestFactory.CreateTestBooking(
            configuration: new BookingConfiguration(BookingModality.Matchmaking, MatchType.Doubles),
            rank: PlayerRank.Intermediate);

        // Act
        var result = booking.AddPlayer(
            Guid.NewGuid(), PlayerRank.Intermediate);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(Error.None, result.Error);
        Assert.Equal(2, booking.Players.Count);
    }

    [Fact]
    public void AddPlayer_ToDirectBooking_ShouldFail()
    {
        // Arrange
        var booking = BookingTestFactory.CreateTestBooking(
            configuration: new BookingConfiguration(BookingModality.Direct, MatchType.Doubles),
            rank: PlayerRank.Intermediate);

        // Act
        var result = booking.AddPlayer(
            Guid.NewGuid(), PlayerRank.Intermediate);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(BookingErrors.OnlyMatchmakingCanAddPlayers, result.Error);
    }

    [Theory]
    [InlineData(MatchType.Singles, 2)]
    [InlineData(MatchType.Doubles, 4)]
    public void Booking_ShouldRespectMaxPlayers(MatchType matchType, int maxPlayers)
    {
        // Arrange
        var booking = BookingTestFactory.CreateTestBooking(
            configuration: new BookingConfiguration(BookingModality.Matchmaking, matchType),
            rank: PlayerRank.Intermediate);

        // Act
        for (int i = 1; i < maxPlayers; i++)
        {
            var result = booking.AddPlayer(
                Guid.NewGuid(), PlayerRank.Intermediate);
            Assert.True(result.IsSuccess);
        }

        // Next should fail
        var shouldFail = booking.AddPlayer(
            Guid.NewGuid(), PlayerRank.Intermediate);

        // Assert
        Assert.False(shouldFail.IsSuccess);
        Assert.Equal(BookingErrors.BookingNotWaiting, shouldFail.Error);
    }
}
