using Bookings.Application;
using Bookings.Application.DTOs.Requests;
using Bookings.Domain;
using Bookings.Tests.Unit.Domain.TestFactories;
using Moq;

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
        var request = new CreateBookingRequest(
            CourtId: Guid.NewGuid(),
            StartTime: DateTime.UtcNow.AddHours(1),
            EndTime: DateTime.UtcNow.AddHours(2),
            Modality: "Direct",
            GameType: "Singles",
            PlayerRank: "Intermediate",
            CourtPricePerHour: 20.0m);

        var result = await _service.CreateBookingAsync(
            userId: Guid.NewGuid(),
            request: request);

        // Assert
        Assert.NotNull(capturedBooking);
        Assert.NotEqual(Guid.Empty, result);
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Booking>()), Times.Once);
    }

    [Fact]
    public async Task CreateBookingAsync_InvalidModality_ShouldThrowException()
    {
        // Arrange
        var request = new CreateBookingRequest(
            CourtId: Guid.NewGuid(),
            StartTime: DateTime.UtcNow.AddHours(1),
            EndTime: DateTime.UtcNow.AddHours(2),
            Modality: "INVALID_MODALITY",
            GameType: "Singles",
            PlayerRank: "Intermediate",
            CourtPricePerHour: 20.0m);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.CreateBookingAsync(
                userId: Guid.NewGuid(),
                request: request));
    }

    [Fact]
    public async Task AddPlayerAsync_ValidBooking_ShouldReturnTrue()
    {
        // Arrange
        var bookingId = Guid.NewGuid();
        var existingBooking = BookingTestFactory.CreateTestBooking(
            configuration: new BookingConfiguration(BookingModality.Matchmaking, GameType.Singles),
            rank: PlayerRank.Intermediate);

        var request = new AddPlayerRequest(
            PlayerId: Guid.NewGuid(),
            PlayerRank: "Intermediate");

        _mockRepository.Setup(r => r.GetByIdAsync(bookingId))
            .ReturnsAsync(existingBooking);

        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Booking>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.AddPlayerAsync(
            bookingId: bookingId,
            request: request);

        // Assert
        Assert.True(result);
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Booking>()), Times.Once);
    }

    [Fact]
    public async Task AddPlayerAsync_BookingNotFound_ShouldReturnFalse()
    {
        // Arrange
        var request = new AddPlayerRequest(
            PlayerId: Guid.NewGuid(),
            PlayerRank: "Intermediate");

        _mockRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Booking?)null);

        // Act
        var result = await _service.AddPlayerAsync(
            bookingId: Guid.NewGuid(),
            request: request);

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

        var request = new AddPlayerRequest(
            PlayerId: Guid.NewGuid(),
            PlayerRank: "Intermediate");

        _mockRepository.Setup(r => r.GetByIdAsync(bookingId))
            .ReturnsAsync(directBooking);

        // Act
        var result = await _service.AddPlayerAsync(
            bookingId: bookingId,
            request: request);

        // Assert
        Assert.False(result);
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Booking>()), Times.Never);
    }
}
