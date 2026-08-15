using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ToDoApp.Data.Shared.JoinEntities;

public static class JoinEntityConfiguration
{
    extension<TJoinEntity, TLeftEntity, TRightEntity>(EntityTypeBuilder<TJoinEntity> thisBuilder)
        where TJoinEntity : class, IJoinEntity<TJoinEntity, TLeftEntity, TRightEntity>
        where TLeftEntity : class
        where TRightEntity : class
    {
        public void ConfigureJoinEntity(
            Expression<Func<TLeftEntity, IEnumerable<TJoinEntity>?>> leftWithMany,
            Expression<Func<TRightEntity, IEnumerable<TJoinEntity>?>> rightWithMany,
            DeleteBehavior deleteBehavior = DeleteBehavior.Cascade
        )
        {
            thisBuilder.HasKey(e => new { e.LeftId, e.RightId });
            thisBuilder.HasOne(e => e.Left).WithMany(leftWithMany).IsRequired().OnDelete(deleteBehavior);
            thisBuilder.HasOne(e => e.Right).WithMany(rightWithMany).IsRequired().OnDelete(deleteBehavior);
        }
    }
}