using Bookings.Application.DTOs.Requests;
using Bookings.Application.DTOs.Responses;
using Bookings.Common;
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

        if (!Enum.TryParse<GameType>(request.GameType, out var parsedGameType))
        {
            throw new ArgumentException($"Invalid game type: {request.GameType}");
        }

        if (!Enum.TryParse<PlayerRank>(request.PlayerRank, out var parsedPlayerRank))
        {
            throw new ArgumentException($"Invalid player rank: {request.PlayerRank}");
        }

        var period = new Period(request.StartTime, request.EndTime);
        var config = new BookingConfiguration(parsedModality, parsedGameType);

        // NOTE: Currency should be configurable by deploy
        // region or user preference in a production ready app
        var booking = Booking.Create(
            userId,
            request.CourtId,
            config,
            period,
            parsedPlayerRank,
            request.CourtPricePerHour,
            Currency.EUR);

        await _repository.AddAsync(booking);
        return booking.Id;
    }

    public async Task<bool> AddPlayerAsync(
        Guid bookingId,
        AddPlayerRequest request)
    {
        if (!Enum.TryParse<PlayerRank>(request.PlayerRank, out var parsedPlayerRank))
        {
            throw new ArgumentException($"Invalid player rank: {request.PlayerRank}");
        }

        var booking = await _repository.GetByIdAsync(bookingId);

        if (booking == null)
        {
            return false;
        }

        var result = booking.AddPlayer(request.PlayerId, parsedPlayerRank);

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

        return booking.ToResponse();
    }
}
