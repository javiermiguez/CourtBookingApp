namespace Users.Domain;

public interface IUserProfileRepository
{
    Task<UserProfile?> GetByIdAsync(Guid id);
    Task<IEnumerable<UserProfile>> GetByUserIdAsync(Guid userId);
    Task AddAsync(UserProfile profile);
    Task UpdateAsync(UserProfile profile);
}
