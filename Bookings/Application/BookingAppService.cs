using Bookings.Application.DTOs.Requests;
using Bookings.Application.DTOs.Responses;
using Bookings.Common;
using Bookings.Domain;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Bookings.Application;

public class BookingAppService
{
    private readonly IBookingRepository _repository;

    public BookingAppService(IBookingRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<Guid>> CreateBookingAsync(
        Guid userId,
        CreateBookingRequest request)
    {
        var modalityResult = EnumParser.TryParse<BookingModality>(request.Modality);
        if (!modalityResult.IsSuccess) return Result.Failure<Guid>(modalityResult.Error);

        var gameTypeResult = EnumParser.TryParse<GameType>(request.GameType);
        if (!gameTypeResult.IsSuccess) return Result.Failure<Guid>(gameTypeResult.Error);

        var playerRankResult = EnumParser.TryParse<PlayerRank>(request.PlayerRank);
        if (!playerRankResult.IsSuccess) return Result.Failure<Guid>(playerRankResult.Error);

        Booking booking;

        try
        {
            var period = new Period(request.StartTime, request.EndTime);
            var config = new BookingConfiguration(modalityResult.Value, gameTypeResult.Value);

            // NOTE: Currency should be configurable by deploy
            // region or user preference in a production ready app
            booking = Booking.Create(
                userId,
                request.CourtId,
                config,
                period,
                playerRankResult.Value,
                request.CourtPricePerHour,
                Currency.EUR);
        }
        catch (DomainException ex)
        {
            return Result.Failure<Guid>(ApplicationErrors.DomainError(ex.Message));
        }
        catch (ArgumentException ex)
        {
            return Result.Failure<Guid>(ApplicationErrors.InvalidBookingData(ex.Message));
        }

        try
        {
            await _repository.AddAsync(booking);
        }
        catch (DbUpdateException ex)
        {
            Debug.WriteLine($"Database error while creating booking: {ex.Message}");
            return Result.Failure<Guid>(ApplicationErrors.DatabaseError());
        }

        return Result.Success(booking.Id);
    }

    public async Task<Result> AddPlayerAsync(
        Guid bookingId,
        AddPlayerRequest request)
    {
        var playerRankResult = EnumParser.TryParse<PlayerRank>(request.PlayerRank);
        if (!playerRankResult.IsSuccess) return Result.Failure(playerRankResult.Error);

        var booking = await _repository.GetByIdAsync(bookingId);
        if (booking == null)
            return Result.Failure(ApplicationErrors.BookingNotFound(bookingId));

        var result = booking.AddPlayer(request.PlayerId, playerRankResult.Value);

        if (!result.IsSuccess)
        {
            return result.Error.Code switch
            {
                nameof(BookingErrors.OnlyMatchmakingCanAddPlayers) =>
                    Result.Failure(ApplicationErrors.DomainError(result.Error.Message)),
                nameof(BookingErrors.BookingNotWaiting) =>
                    Result.Failure(ApplicationErrors.DomainError(result.Error.Message)),
                nameof(BookingErrors.InvalidPlayerRank) =>
                    Result.Failure(ApplicationErrors.DomainError(result.Error.Message)),
                nameof(BookingErrors.PlayerAlreadyInBooking) =>
                    Result.Failure(ApplicationErrors.DomainError(result.Error.Message)),
                _ => Result.Failure(result.Error)
            };
        }

        try
        {
            await _repository.UpdateAsync(booking);
        }
        catch (DbUpdateException ex)
        {
            Debug.WriteLine($"Database error while adding a player: {ex.Message}");
            return Result.Failure(ApplicationErrors.DatabaseError());
        }

        return Result.Success();
    }

    public async Task<Result<BookingResponse>> GetBookingResponseAsync(Guid id)
    {
        var booking = await _repository.GetByIdAsync(id);

        if (booking == null)
        {
            return Result.Failure<BookingResponse>(ApplicationErrors.BookingNotFound(id));
        }

        return Result.Success(booking.ToResponse());
    }
}
