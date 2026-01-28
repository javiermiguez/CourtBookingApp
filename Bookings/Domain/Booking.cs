namespace Bookings.Domain;

// Value Object - BookingConfiguration
public record BookingConfiguration(BookingMode Mode, MatchType MatchType);

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

        Status = configuration.Mode == BookingMode.Matchmaking
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

    public bool CanPlayerJoin(PlayerRank rank)
    {
        var requester = _players.FirstOrDefault(p => p.IsRequester);

        if (requester == null)
        {
            throw new DomainException("Can't assess if the player can join because no requester was found in the booking.");
        }

        if (Configuration.Mode != BookingMode.Matchmaking)
        {
            return false;
        }

        if (Status != BookingStatus.WaitingForPlayers)
        {
            return false;
        }

        if (_players.Count >= GetMaxPlayers())
        {
            return false;
        }

        return requester.Rank == rank;
    }

    public void AddPlayer(Guid userId, PlayerRank rank)
    {
        if (Configuration.Mode != BookingMode.Matchmaking)
        {
            throw new DomainException("Only matchmaking bookings can add players");
        }

        if (_players.Any(p => p.UserId == userId))
        {
            throw new DomainException("Player already in booking");
        }

        if (!CanPlayerJoin(rank))
        {
            throw new DomainException("Player cannot join the booking.");
        }

        var player = new Player(userId, rank, false);
        _players.Add(player);

        if (_players.Count >= GetMaxPlayers())
            Status = BookingStatus.PendingPayment;
    }

    private int GetMaxPlayers() => Configuration.MatchType switch
    {
        MatchType.Singles => 2,
        MatchType.Doubles => 4,
        _ => throw new InvalidOperationException("Unknown match type")
    };
}
