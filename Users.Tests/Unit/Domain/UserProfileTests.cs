using Users.Domain;
using Users.Tests.Unit.Domain.TestFactories;

namespace Users.Tests.Unit.Domain;

public class UserProfileTests
{
    [Fact]
    public void Create_UserProfile_ShouldInitializeCorrectly()
    {
        var userId = Guid.NewGuid();
        var firstName = "Javier";

        var profile = UserProfileTestFactory.CreateTestProfile(userId: userId, firstName: firstName);

        Assert.Equal(userId, profile.UserId);
        Assert.Equal("Javier", profile.Name.FirstName);
        Assert.Equal(PlayerRank.Beginner, profile.PlayerRank);
    }

    [Theory]
    [InlineData(UserType.Player, true)]
    [InlineData(UserType.CourtOwner, false)]
    public void IsPlayer_ShouldReturnExpectedValue(UserType type, bool expectedIsPlayer)
    {
        var profile = UserProfileTestFactory.CreateTestProfile(type: type);

        var result = profile.IsPlayer();

        Assert.Equal(expectedIsPlayer, result);
    }
}
