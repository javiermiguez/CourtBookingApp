using Users.Common;

namespace Users.Domain;

public static class UserErrors
{
    public static readonly Error ProfileTypeAlreadyExists = new(
        "User.ProfileTypeAlreadyExists",
        "The user already has a profile of this type",
        ErrorType.Conflict);

    public static readonly Error ProfileNotFound = new(
        "User.ProfileNotFound",
        "The requested user profile was not found",
        ErrorType.NotFound);
}
