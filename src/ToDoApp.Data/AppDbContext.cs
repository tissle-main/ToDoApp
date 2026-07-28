using Microsoft.EntityFrameworkCore;
using ToDoApp.Data.Features.Auth.Users;
using ToDoApp.Data.Features.Auth.Roles;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace ToDoApp.Data;

public sealed class AppDbContext(
    DbContextOptions<AppDbContext> options
) : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>(options)
{
    #region Base
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
    #endregion
}