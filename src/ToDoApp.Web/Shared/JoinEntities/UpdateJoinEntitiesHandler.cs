using ErrorOr;
using Mediator;
using ToDoApp.Data;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Data.Shared.JoinEntities;
using ToDoApp.Data.Features.Auth.Users;
using ToDoApp.Data.Shared.KeyedEntities;
using ToDoApp.Web.Shared.Behaviors.Authorized;
using ToDoApp.Data.Features.Auth.Users.ForeignKey;

namespace ToDoApp.Web.Shared.JoinEntities;

public abstract class UpdateJoinEntitiesHandler<TMessage, TJoinEntity, TLeftEntity, TRightEntity>(
    AppDbContext thisDbContext
) where TMessage :  IUpdateJoinEntitiesMessage<TJoinEntity, TLeftEntity, TRightEntity>
  where TJoinEntity : class, IJoinEntity<TJoinEntity, TLeftEntity, TRightEntity>
  where TLeftEntity : class, IKeyedEntity, IUserEntityForeignKey
  where TRightEntity : class, IKeyedEntity, IUserEntityForeignKey
{
    #region Instance
    private DbSet<TJoinEntity> JoinEntities { get; } = thisDbContext.Set<TJoinEntity>();
    private DbSet<TLeftEntity> LeftEntities { get; } = thisDbContext.Set<TLeftEntity>();
    private DbSet<TRightEntity> RightEntities { get; } = thisDbContext.Set<TRightEntity>();
    #endregion

    #region Interfaces
    public async ValueTask<ErrorOr<Unit>> Handle(TMessage message, CancellationToken cancellationToken)
    {
        if(message.OldEntities.Count == 0 && message.NewEntities.Count == 0)
        {
            return Unit.Value;
        }
        UserEntity user = message.User;
        if(message.OldEntities.Count > 0)
        {
            TJoinEntity[] jes = await JoinEntities.Include(je => je.Left).Include(je => je.Right).Where(
                je => je.Left!.UserId == user.Id && je.Right!.UserId == user.Id
            ).ToArrayAsync(cancellationToken);
            if(message.OldEntities.Except(jes).Any())
            {
                return Error.Unexpected();
            }
        }
        if(message.NewEntities.Count > 0)
        {
            Guid[] ids = message.NewEntities.Select(e => e.LeftId).Distinct().ToArray();
            TLeftEntity[] left = await LeftEntities.Where(e => ids.Contains(e.Id)).ToArrayAsync(cancellationToken);
            if(left.Any(e => e.UserId != user.Id))
            {
                return Error.Forbidden();
            }
            if(ids.Length != left.Length)
            {
                return Error.NotFound();
            }

            ids = message.NewEntities.Select(e => e.RightId).Distinct().ToArray();
            TRightEntity[] right = await RightEntities.Where(e => ids.Contains(e.Id)).ToArrayAsync(cancellationToken);
            if(right.Any(e => e.UserId != user.Id))
            {
                return Error.Forbidden();
            }
            if(ids.Length != right.Length)
            {
                return Error.NotFound();
            }
        }
        JoinEntities.RemoveRange(message.OldEntities.Except(message.NewEntities));
        await JoinEntities.AddRangeAsync(message.NewEntities.Except(message.OldEntities), cancellationToken);
        await thisDbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
    #endregion
}