using ToDoApp.Data.Features.Auth;
using ToDoApp.Data.Features.Tasks;
using ToDoApp.Data.Shared.Entities;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Data.Features.Categories;
using ToDoApp.Data.Features.Tasks_Categories;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace ToDoApp.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<UserEntity, RoleEntity, Guid>(options)
{
    #region Instance
    public DbSet<RefreshTokenEntity> RefreshTokens { get; set; } = null!; //Init by EFCore
    public DbSet<TaskEntity> Tasks { get; set; } = null!; //Init by EFCore
    public DbSet<CategoryEntity> Categories { get; set; } = null!; //Init by EFCore
    public DbSet<Task_Category_JoinEntity> Tasks_Categories { get; set; } = null!; //Init by EFCore
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
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }
    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        this.GenerateIdForKeyedEntities();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }
    #endregion
}