using Bookings.Domain;

namespace Bookings.Tests.Unit.Domain;

public class PeriodTests
{
    [Fact]
    public void Create_ValidPeriod_ShouldInitializeCorrectly()
    {
        // Arrange
        var start = new DateTime(2024, 1, 1, 10, 0, 0);
        var end = new DateTime(2024, 1, 1, 12, 0, 0);

        // Act
        var period = new Period(start, end);

        // Assert
        Assert.Equal(start, period.Start);
        Assert.Equal(end, period.End);
        Assert.Equal(TimeSpan.FromHours(2), period.Duration);
    }

    [Fact]
    public void Create_PeriodWithSameStartAndEnd_ShouldThrowException()
    {
        // Arrange
        var sameTime = new DateTime(2024, 1, 1, 10, 0, 0);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new Period(sameTime, sameTime));
    }

    [Fact]
    public void Create_PeriodWithEndBeforeStart_ShouldThrowException()
    {
        // Arrange
        var start = new DateTime(2024, 1, 1, 12, 0, 0);
        var end = new DateTime(2024, 1, 1, 10, 0, 0);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new Period(start, end));
    }
}
