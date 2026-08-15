using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ToDoApp.Data.Features.Auth.Users.ForeignKey;

public static class UserEntityForeignKeyConfiguration
{
    extension<TEntity>(EntityTypeBuilder<TEntity> thisBuilder) where TEntity : class, IUserEntityForeignKey
    {
        public void ConfigureUserEntityForeignKey(
            Expression<Func<UserEntity, IEnumerable<TEntity>?>> withMany,
            bool isRequired = true,
            DeleteBehavior deleteBehavior = DeleteBehavior.Restrict
        )
        {
            thisBuilder.HasOne(e => e.User).WithMany(withMany).IsRequired(isRequired).HasForeignKey(e => e.UserId).OnDelete(deleteBehavior);
        }
    }
}