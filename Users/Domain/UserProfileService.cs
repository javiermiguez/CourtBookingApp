using Users.Common;

namespace Users.Domain;

public class UserProfileService(IUserProfileRepository repository)
{
    public async Task<Result<UserProfile>> CreateProfileAsync(Guid userId, UserType type, FullName name)
    {
        var existingProfiles = await repository.GetByUserIdAsync(userId);

        if (!CanAddProfile(existingProfiles, type))
            return new Result<UserProfile>(null!, false, UserErrors.ProfileTypeAlreadyExists);

        var newProfile = new UserProfile(userId, type, name);

        await repository.AddAsync(newProfile);

        return new Result<UserProfile>(newProfile, true, Error.None);
    }

    public bool CanAddProfile(IEnumerable<UserProfile> existingProfiles, UserType newProfileType)
    {
        // Esta é a túa regra de ouro: non repetir tipos de perfil
        return !existingProfiles.Any(p => p.Type == newProfileType);
    }
}
