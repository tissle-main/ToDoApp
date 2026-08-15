using ToDoApp.Data.Features.Tasks;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Data.Features.Categories;
using ToDoApp.Data.Features.Auth.Users;
using ToDoApp.Data.Features.Auth.Roles;
using ToDoApp.Data.Shared.KeyedEntities;
using ToDoApp.Data.Features.Tasks_Categories;
using ToDoApp.Data.Features.Auth.RefreshTokens;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace ToDoApp.Data;

public sealed class AppDbContext : IdentityDbContext<UserEntity, RoleEntity, Guid>
{
    #region Instance
    public DbSet<RefreshTokenEntity> RefreshTokens { get; set; } = null!; //Init by EFCore
    public DbSet<TaskEntity> Tasks { get; set; } = null!; //Init by EFCore
    public DbSet<CategoryEntity> Categories { get; set; } = null!; //Init by EFCore
    public DbSet<Task_Category_JoinEntity> Tasks_Categories { get; set; } = null!; //Init by EFCore

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        base.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }
    #endregion

    #region Base
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        this.GenerateIdForKeyedEntities();
        int changedNumber = base.SaveChanges(acceptAllChangesOnSuccess);
        base.ChangeTracker.Clear();
        return changedNumber;

    }
    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        this.GenerateIdForKeyedEntities();
        int changedNumber = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        base.ChangeTracker.Clear();
        return changedNumber;
    }
    #endregion
}