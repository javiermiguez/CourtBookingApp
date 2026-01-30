using Bookings.Domain;

namespace Bookings.Application;

public class BookingAppService
{
    private readonly IBookingRepository _repository;

    public BookingAppService(IBookingRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> CreateBookingAsync(
        Guid userId,
        Guid courtId,
        DateTime startTime,
        DateTime endTime,
        string modality,
        string gameType,
        string playerRank,
        decimal courtPricePerHour)
    {
        if (!Enum.TryParse<BookingModality>(modality, out var parsedModality))
            throw new ArgumentException("Invalid modality");

        var period = new Period(startTime, endTime);
        var config = new BookingConfiguration(
            parsedModality,
            Enum.Parse<GameType>(gameType));

        var booking = Booking.Create(
            userId,
            courtId,
            config,
            period,
            Enum.Parse<PlayerRank>(playerRank),
            courtPricePerHour,
            Currency.EUR);

        await _repository.AddAsync(booking);

        return booking.Id;
    }

    public async Task<bool> AddPlayerAsync(
        Guid bookingId,
        Guid playerId,
        string playerRank)
    {
        var booking = await _repository.GetByIdAsync(bookingId);
        if (booking == null)
            return false;

        var result = booking.AddPlayer(
            playerId,
            Enum.Parse<PlayerRank>(playerRank));

        if (!result.IsSuccess)
            return false;

        await _repository.UpdateAsync(booking);

        return true;
    }

    public async Task<Booking?> GetBookingAsync(Guid id)
    {
        return await _repository.GetByIdAsync(id);
    }
}
