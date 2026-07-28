using ToDoApp.Data.Features.Tasks;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Data.Features.Auth.Users;
using ToDoApp.Data.Features.Auth.Roles;
using ToDoApp.Data.Features.Categories;
using ToDoApp.Data.Features.Tasks_Categories;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace ToDoApp.Data;

public sealed class AppDbContext(
    DbContextOptions<AppDbContext> options
) : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>(options)
{
    #region Instance
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
    #endregion
}