using Microsoft.EntityFrameworkCore;
using Users.Application;
using Users.Domain;

namespace Users.Infrastructure;

public class UserProfileRepository : IUserProfileRepository
{
    private readonly UsersDbContext _context;

    public UserProfileRepository(UsersDbContext context)
    {
        _context = context;
    }

    public async Task<UserProfile?> GetByIdAsync(Guid id)
    {
        // Buscamos polo ID da entidade perfil
        return await _context.UserProfiles
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<UserProfile>> GetByUserIdAsync(Guid userId)
    {
        // Como un usuario pode ter varios perfís, devolvemos a lista filtrada
        return await _context.UserProfiles
            .Where(p => p.UserId == userId)
            .ToListAsync();
    }

    public async Task AddAsync(UserProfile profile)
    {
        await _context.UserProfiles.AddAsync(profile);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(UserProfile profile)
    {
        _context.UserProfiles.Update(profile);
        await _context.SaveChangesAsync();
    }
}
