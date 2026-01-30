using Bookings.Application;
using Bookings.Domain;
using Bookings.Tests.Unit.Domain.TestFactories;
using Moq;
using Xunit;

namespace Bookings.Tests.Unit.Application;

public class BookingAppServiceTests
{
    private readonly Mock<IBookingRepository> _mockRepository;
    private readonly BookingAppService _service;

    public BookingAppServiceTests()
    {
        _mockRepository = new Mock<IBookingRepository>();
        _service = new BookingAppService(_mockRepository.Object);
    }

    [Fact]
    public async Task CreateBookingAsync_ValidData_ShouldReturnBookingId()
    {
        // Arrange
        var capturedBooking = default(Booking);

        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Booking>()))
            .Callback<Booking>(b => capturedBooking = b)
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.CreateBookingAsync(
            userId: Guid.NewGuid(),
            courtId: Guid.NewGuid(),
            startTime: DateTime.UtcNow.AddHours(1),
            endTime: DateTime.UtcNow.AddHours(2),
            modality: "Direct",
            gameType: "Singles",
            playerRank: "Intermediate",
            courtPricePerHour: 20.0m);

        // Assert
        Assert.NotNull(capturedBooking);
        Assert.NotEqual(Guid.Empty, result);
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Booking>()), Times.Once);
    }

    [Fact]
    public async Task CreateBookingAsync_InvalidModality_ShouldThrowException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.CreateBookingAsync(
                userId: Guid.NewGuid(),
                courtId: Guid.NewGuid(),
                startTime: DateTime.UtcNow.AddHours(1),
                endTime: DateTime.UtcNow.AddHours(2),
                modality: "INVALID_MODALITY",
                gameType: "Singles",
                playerRank: "Intermediate",
                courtPricePerHour: 20.0m));
    }

    [Fact]
    public async Task AddPlayerAsync_ValidBooking_ShouldReturnTrue()
    {
        // Arrange
        var bookingId = Guid.NewGuid();
        var existingBooking = BookingTestFactory.CreateTestBooking(
            configuration: new BookingConfiguration(BookingModality.Matchmaking, GameType.Singles),
            rank: PlayerRank.Intermediate);

        _mockRepository.Setup(r => r.GetByIdAsync(bookingId))
            .ReturnsAsync(existingBooking);

        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Booking>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.AddPlayerAsync(
            bookingId: bookingId,
            playerId: Guid.NewGuid(),
            playerRank: "Intermediate");

        // Assert
        Assert.True(result);
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Booking>()), Times.Once);
    }

    [Fact]
    public async Task AddPlayerAsync_BookingNotFound_ShouldReturnFalse()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Booking?)null); // ← Non existe

        // Act
        var result = await _service.AddPlayerAsync(
            bookingId: Guid.NewGuid(),
            playerId: Guid.NewGuid(),
            playerRank: "Intermediate");

        // Assert
        Assert.False(result);
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Booking>()), Times.Never);
    }

    [Fact]
    public async Task AddPlayerAsync_PlayerCannotJoin_ShouldReturnFalse()
    {
        // Arrange
        var bookingId = Guid.NewGuid();

        var directBooking = BookingTestFactory.CreateTestBooking(
            configuration: new BookingConfiguration(BookingModality.Direct, GameType.Singles));

        _mockRepository.Setup(r => r.GetByIdAsync(bookingId))
            .ReturnsAsync(directBooking);

        // Act
        var result = await _service.AddPlayerAsync(
            bookingId: bookingId,
            playerId: Guid.NewGuid(),
            playerRank: "Intermediate");

        // Assert
        Assert.False(result);
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Booking>()), Times.Never);
    }
}
