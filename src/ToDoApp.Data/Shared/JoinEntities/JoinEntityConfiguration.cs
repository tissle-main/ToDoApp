using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ToDoApp.Data.Shared.JoinEntities;

public static class JoinEntityConfiguration
{
    public static void ConfigureJoinEntity<TJoinEntity, TLeftEntity, TRightEntity>(
        this EntityTypeBuilder<TJoinEntity> builder,
        Expression<Func<TLeftEntity, IEnumerable<TJoinEntity>?>> leftWithMany,
        Expression<Func<TRightEntity, IEnumerable<TJoinEntity>?>> rightWithMany,
        DeleteBehavior deleteBehavior = DeleteBehavior.Cascade
    ) where TJoinEntity : class, IJoinEntity<TLeftEntity, TRightEntity>
      where TLeftEntity : class
      where TRightEntity : class
    {
        builder.HasKey(e => new { e.LeftId, e.RightId });
        builder.HasOne(e => e.Left).WithMany(leftWithMany).IsRequired().OnDelete(deleteBehavior);
        builder.HasOne(e => e.Right).WithMany(rightWithMany).IsRequired().OnDelete(deleteBehavior);
    }
}