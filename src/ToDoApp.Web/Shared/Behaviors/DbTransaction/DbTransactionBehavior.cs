using ErrorOr;
using Mediator;
using ToDoApp.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace ToDoApp.Web.Shared.Behaviors.DbTransaction;

public sealed class DbTransactionBehavior<TMessage, TErrorOrValue>(AppDbContext thisDbContext) : IPipelineBehavior<TMessage, TErrorOrValue>
    where TMessage : IDbTransactionBehaviorMessage
    where TErrorOrValue : IErrorOr
{
    #region Interfaces
    public async ValueTask<TErrorOrValue> Handle(TMessage message, MessageHandlerDelegate<TMessage, TErrorOrValue> next, CancellationToken cancellationToken)
    {
        if(!message.BeginDbTransaction)
        {
            return await next(message, cancellationToken);
        }
        await using IDbContextTransaction transaction = await thisDbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            TErrorOrValue errorOrValue = await next(message, cancellationToken);
            if(errorOrValue.IsSuccess)
            {
                await transaction.CommitAsync(cancellationToken);
            }
            else
            {
                await transaction.RollbackAsync(cancellationToken);
            }
            return errorOrValue;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
    #endregion
}