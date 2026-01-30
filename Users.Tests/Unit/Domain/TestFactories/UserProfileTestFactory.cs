using Users.Domain;

namespace Users.Tests.Unit.Domain.TestFactories;

public static class UserProfileTestFactory
{
    public static UserProfile CreateTestProfile(
        Guid? userId = null,
        UserType type = UserType.Player,
        string firstName = "Miro",
        string lastName = "Pereira",
        PlayerRank? rank = PlayerRank.Beginner)
    {
        return new UserProfile(
            userId ?? Guid.NewGuid(),
            type,
            new FullName(firstName, lastName),
            rank
        );
    }
}
