namespace Users.Infrastructure;

public class User
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;

    private readonly List<Guid> _profileIds = new();
    public IReadOnlyList<Guid> ProfileIds => _profileIds.AsReadOnly();

    private User() { }

    public User(string email)
    {
        Id = Guid.NewGuid();
        Email = email;
    }

    public void AddProfile(Guid profileId)
    {
        if (!_profileIds.Contains(profileId))
            _profileIds.Add(profileId);
    }

    public void RemoveProfile(Guid profileId)
    {
        _profileIds.Remove(profileId);
    }
}
