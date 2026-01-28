using Bookings.Common;

namespace Bookings.Domain;

// Value Object - BookingConfiguration
public record BookingConfiguration(BookingModality Modality, GameType GameType);

// Aggregate Root - Booking
public class Booking
{
    public record Player(Guid UserId, PlayerRank Rank, bool IsRequester);

    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public Guid CourtId { get; private set; }
    public BookingConfiguration Configuration { get; private set; } = default!;
    public BookingStatus Status { get; private set; } = default!;
    public Period RequestPeriod { get; private set; } = default!;
    public Period BookingPeriod { get; private set; } = default!;
    public Price Price { get; private set; } = default!;

    public IReadOnlyList<Player> Players => _players.AsReadOnly();
    private List<Player> _players = new();

    private Booking()
    {
    }

    private Booking(
        Guid id,
        Guid userId,
        Guid courtId,
        BookingConfiguration configuration,
        Period bookingPeriod,
        PlayerRank userRank,
        decimal courtPricePerHour,
        Currency currency)
    {
        if (courtPricePerHour <= 0)
        {
            throw new DomainException("Price must be positive");
        }

        if (bookingPeriod.Start < DateTime.UtcNow)
        {
            throw new DomainException("Booking start time cannot be in the past");
        }

        Id = id;
        UserId = userId;
        CourtId = courtId;
        Configuration = configuration;

        Status = configuration.Modality == BookingModality.Matchmaking
            ? BookingStatus.WaitingForPlayers
            : BookingStatus.Pending;

        RequestPeriod = new Period(
            DateTime.UtcNow,
            DateTime.UtcNow.AddMinutes(10));
        BookingPeriod = bookingPeriod;
        Price = CalculatePrice(courtPricePerHour, currency);

        // Add the requester as a player
        _players.Add(new Player(userId, userRank, true));
    }

    public static Booking Create(
        Guid userId,
        Guid courtId,
        BookingConfiguration configuration,
        Period bookingPeriod,
        PlayerRank userRank,
        decimal courtPricePerHour,
        Currency currency)
    {
        return new Booking(
            id: Guid.NewGuid(),
            userId: userId,
            courtId: courtId,
            configuration: configuration,
            bookingPeriod: bookingPeriod,
            userRank: userRank,
            courtPricePerHour: courtPricePerHour,
            currency: currency);
    }

    private Price CalculatePrice(decimal courtPricePerHour, Currency currency)
    {
        var hours = BookingPeriod.Duration().TotalHours;
        return new Price((decimal)hours * courtPricePerHour, currency);
    }

    public Result AddPlayer(Guid userId, PlayerRank rank)
    {
        var playerCanJoinResult = PlayerCanJoin(userId, rank);

        if (!playerCanJoinResult.IsSuccess)
        {
            return playerCanJoinResult;
        }

        _players.Add(new Player(userId, rank, false));

        if (_players.Count >= GetMaxPlayers())
        {
            Status = BookingStatus.PendingPayment;
        }

        return Result.Success();
    }

    public Result PlayerCanJoin(Guid userId, PlayerRank rank)
    {
        var requester = _players.FirstOrDefault(p => p.IsRequester);

        if (requester == null)
        {
            throw new DomainException("No requester in booking");
        }

        if (Configuration.Modality != BookingModality.Matchmaking)
        {
            return Result.Failure(BookingErrors.OnlyMatchmakingCanAddPlayers);
        }

        if (Status != BookingStatus.WaitingForPlayers)
        {
            return Result.Failure(BookingErrors.BookingNotWaiting);
        }

        if (requester.Rank != rank)
        {
            return Result.Failure(BookingErrors.InvalidPlayerRank);
        }

        if (_players.Any(p => p.UserId == userId))
        {
            return Result.Failure(BookingErrors.PlayerAlreadyInBooking);
        }

        return Result.Success();
    }

    private int GetMaxPlayers() => Configuration.GameType switch
    {
        GameType.Singles => 2,
        GameType.Doubles => 4,
        _ => throw new InvalidOperationException("Unknown game type")
    };
}
