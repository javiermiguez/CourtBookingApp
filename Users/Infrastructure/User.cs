namespace Users.Infrastructure;

public class User
{
    public Guid Id { get; private set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;

    private User() { }

    public User(string email)
    {
        Id = Guid.NewGuid();
        Email = email;
    }
}
