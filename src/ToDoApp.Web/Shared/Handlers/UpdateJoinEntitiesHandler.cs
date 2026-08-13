using ErrorOr;
using Mediator;
using ToDoApp.Data;
using ToDoApp.Data.Shared.Entities;
using ToDoApp.Web.Shared.Behaviors;
using Microsoft.EntityFrameworkCore;

namespace ToDoApp.Web.Shared.Handlers;

public abstract class UpdateJoinEntitiesHandler<TMessage, TJoinEntity, TLeftEntity, TRightEntity>(
    AppDbContext thisDbContext,
    Error invalidLeftIds,
    Error invalidRightIds
) where TMessage :  IUpdateJoinEntitiesMessage<TJoinEntity, TLeftEntity, TRightEntity>
  where TJoinEntity : class, IJoinEntity<TJoinEntity, TLeftEntity, TRightEntity>
  where TLeftEntity : class, IKeyedEntity
  where TRightEntity : class, IKeyedEntity
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
        if(message.OldEntities.Count > 0)
        {
            int count = await JoinEntities.AsNoTracking().CountAsync(e => message.OldEntities.Contains(e), cancellationToken);
            if(count != message.OldEntities.Count)
            {
                return Error.Unexpected();
            }
        }
        if(message.NewEntities.Count > 0)
        {
            Guid[] ids = message.NewEntities.Select(e => e.LeftId).Distinct().ToArray();
            int count = await LeftEntities.AsNoTracking().CountAsync(e => ids.Contains(e.Id), cancellationToken);
            if(count != ids.Length)
            {
                return invalidLeftIds;
            }

            ids = message.NewEntities.Select(e => e.RightId).Distinct().ToArray();
            count = await RightEntities.AsNoTracking().CountAsync(e => ids.Contains(e.Id), cancellationToken);
            if(count != ids.Length)
            {
                return invalidRightIds;
            }
        }
        JoinEntities.RemoveRange(message.OldEntities.Except(message.NewEntities));
        await JoinEntities.AddRangeAsync(message.NewEntities.Except(message.OldEntities), cancellationToken);
        return Unit.Value;
    }
    #endregion
}
public interface IUpdateJoinEntitiesMessage<TJoinEntity, TLeftEntity, TRightEntity> : IDbSaveMessage
    where TJoinEntity : class, IJoinEntity<TJoinEntity, TLeftEntity, TRightEntity>
    where TLeftEntity : class, IKeyedEntity
    where TRightEntity : class, IKeyedEntity
{
    public IReadOnlyCollection<TJoinEntity> OldEntities { get; }
    public IReadOnlyCollection<TJoinEntity> NewEntities { get; }
}