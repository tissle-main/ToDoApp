using Microsoft.EntityFrameworkCore;
using ToDoApp.Data.Features.Auth.Roles;
using ToDoApp.Data.Features.Auth.Users;
using ToDoApp.Data.Shared.KeyedEntities;
using ToDoApp.Data.Features.Auth.RefreshTokens;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace ToDoApp.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<UserEntity, RoleEntity, Guid>(options)
{
    #region Instance
    public DbSet<RefreshTokenEntity> RefreshTokens { get; set; } = null!; //Init by EFCore
    #endregion

    #region Base
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        foreach(EntityEntry<IKeyedEntity> entry in base.ChangeTracker.Entries<IKeyedEntity>())
        {
            if(entry.State is EntityState.Added)
            {
                entry.Entity.Id = Guid.CreateVersion7();
            }
        }
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }
    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        foreach(EntityEntry<IKeyedEntity> entry in base.ChangeTracker.Entries<IKeyedEntity>())
        {
            if(entry.State is EntityState.Added)
            {
                entry.Entity.Id = Guid.CreateVersion7();
            }
        }
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }
    #endregion
}