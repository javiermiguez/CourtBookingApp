using Microsoft.EntityFrameworkCore;
using Users.Domain;

namespace Users.Infrastructure;

public class UsersDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();

    public UsersDbContext(DbContextOptions<UsersDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Email).IsRequired();
        });

        modelBuilder.Entity<UserProfile>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.OwnsOne(p => p.Name);
            entity.OwnsOne(p => p.Location);
        });

        base.OnModelCreating(modelBuilder);
    }
}
