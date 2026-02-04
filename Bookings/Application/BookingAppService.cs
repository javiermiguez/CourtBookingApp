using Bookings.Application.DTOs.Requests;
using Bookings.Application.DTOs.Responses;
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
        CreateBookingRequest request)
    {
        if (!Enum.TryParse<BookingModality>(request.Modality, out var parsedModality))
        {
            throw new ArgumentException($"Invalid modality: {request.Modality}");
        }

        var period = new Period(request.StartTime, request.EndTime);
        var config = new BookingConfiguration(
            parsedModality,
            Enum.Parse<GameType>(request.GameType));

        // NOTE: Currency should be configurable by deploy
        // region or user preference in a production ready app
        var booking = Booking.Create(
            userId,
            request.CourtId,
            config,
            period,
            Enum.Parse<PlayerRank>(request.PlayerRank),
            request.CourtPricePerHour,
            Currency.EUR);

        await _repository.AddAsync(booking);
        return booking.Id;
    }

    public async Task<bool> AddPlayerAsync(
        Guid bookingId,
        AddPlayerRequest request)
    {
        var booking = await _repository.GetByIdAsync(bookingId);

        if (booking == null)
        {
            return false;
        }

        var result = booking.AddPlayer(
            request.PlayerId,
            Enum.Parse<PlayerRank>(request.PlayerRank));

        if (!result.IsSuccess)
        {
            return false;
        }

        await _repository.UpdateAsync(booking);
        return true;
    }

    public async Task<BookingResponse?> GetBookingResponseAsync(Guid id)
    {
        var booking = await _repository.GetByIdAsync(id);

        if (booking == null)
        {
            return null;
        }

        return MapToResponse(booking);
    }

    private BookingResponse MapToResponse(Booking booking)
    {
        return new BookingResponse(
            booking.Id,
            booking.UserId,
            booking.CourtId,
            booking.Status.ToString(),
            booking.Configuration.Modality.ToString(),
            booking.Configuration.GameType.ToString(),
            booking.BookingPeriod.Start,
            booking.BookingPeriod.End,
            booking.Price.Amount,
            booking.Price.Currency.ToString(),
            booking.Players.Select(p => new PlayerResponse(
                p.UserId,
                p.Rank.ToString(),
                p.IsRequester)));
    }
}
