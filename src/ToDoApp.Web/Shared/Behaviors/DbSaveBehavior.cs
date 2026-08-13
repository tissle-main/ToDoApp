using ErrorOr;
using Mediator;
using ToDoApp.Data;
using System.Runtime.CompilerServices;

namespace ToDoApp.Web.Shared.Behaviors;

public sealed class DbSaveBehavior<TMessage, TErrorOrValue>(AppDbContext thisDbContext) : IPipelineBehavior<TMessage, TErrorOrValue>
    where TMessage : IDbSaveMessage
    where TErrorOrValue : IErrorOr
{
    #region Interfaces
    public async ValueTask<TErrorOrValue> Handle(TMessage message, MessageHandlerDelegate<TMessage, TErrorOrValue> next, CancellationToken cancellationToken)
    {
        TErrorOrValue errorOrValue = await next(message, cancellationToken);
        if(errorOrValue.IsSuccess && message.SaveDatabase)
        {
            await thisDbContext.SaveChangesAsync(cancellationToken);
        }
        return errorOrValue;
    }
    #endregion
}
public static class DbSaveBehaviorExtensions
{
    private static ConditionalWeakTable<IDbSaveMessage, DbSaveMetadata> MetadataTable { get; } = [];

    private static DbSaveMetadata GetMetadata(IDbSaveMessage message)
    {
        return MetadataTable.GetOrAdd(message, static _ => new DbSaveMetadata());
    }

    extension(IDbSaveMessage thisMessage)
    {
        public bool SaveDatabase
        {
            get => GetMetadata(thisMessage).SaveDatabase;
            set
            {
                GetMetadata(thisMessage).SaveDatabase = value;
            }
        }
    }
}
public sealed class DbSaveMetadata
{
    public bool SaveDatabase { get; set; } = true;
}
public interface IDbSaveMessage : IMessage;