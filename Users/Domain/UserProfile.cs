namespace Users.Domain;

public class UserProfile
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public UserType Type { get; private set; }
    public FullName Name { get; private set; }
    public PlayerRank? PlayerRank { get; private set; }
    public DateTime? BirthDate { get; private set; }
    public string? IdentificationDocument { get; private set; }
    public Location? Location { get; private set; }

    private UserProfile()
    {
        Name = null!;
    }

    public UserProfile(Guid userId, UserType type, FullName name, PlayerRank? rank = null)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        Type = type;
        Name = name;
        PlayerRank = rank;
    }

    public bool IsPlayer() => Type == UserType.Player;

    public void UpdatePersonalInformation(DateTime? birthDate, string? document)
    {
        // Will add some validation logic here
        BirthDate = birthDate;
        IdentificationDocument = document;
    }

    public void UpdateLocation(Location? newLocation)
    {
        Location = newLocation;
    }

    public void UpdateRank(PlayerRank newRank)
    {
        PlayerRank = newRank;
    }
}
