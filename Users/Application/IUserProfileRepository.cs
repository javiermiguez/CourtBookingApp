using Users.Domain;

namespace Users.Application;

public interface IUserProfileRepository
{
    Task<UserProfile?> GetByIdAsync(Guid id);
    Task<IEnumerable<UserProfile>> GetByUserIdAsync(Guid userId);
    Task AddAsync(UserProfile profile);
    Task UpdateAsync(UserProfile profile);
}
