using ToDoApp.Data.Shared.JoinEntities;
using ToDoApp.Data.Shared.KeyedEntities;
using ToDoApp.Web.Shared.Behaviors.Authorized;
using ToDoApp.Web.Shared.Behaviors.DbTransaction;
using ToDoApp.Data.Features.Auth.Users.ForeignKey;

namespace ToDoApp.Web.Shared.JoinEntities;

public interface IUpdateJoinEntitiesMessage<TJoinEntity, TLeftEntity, TRightEntity> : IDbTransactionBehaviorMessage, IAuthorizedBehaviorMessage
    where TJoinEntity : class, IJoinEntity<TJoinEntity, TLeftEntity, TRightEntity>
    where TLeftEntity : class, IKeyedEntity, IUserEntityForeignKey
    where TRightEntity : class, IKeyedEntity, IUserEntityForeignKey
{
    public IReadOnlyCollection<TJoinEntity> OldEntities { get; }
    public IReadOnlyCollection<TJoinEntity> NewEntities { get; }
}