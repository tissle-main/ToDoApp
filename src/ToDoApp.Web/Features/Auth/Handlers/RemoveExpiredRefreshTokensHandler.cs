using ErrorOr;
using Mediator;
using ToDoApp.Data;
using ToDoApp.Data.Features.Auth;
using ToDoApp.Web.Shared.Behaviors;
using Microsoft.EntityFrameworkCore;

namespace ToDoApp.Web.Features.Auth.Handlers;

public sealed class RemoveExpiredRefreshTokensHandler(AppDbContext thisDbContext) : ICommandHandler<RemoveExpiredRefreshTokensCommand, ErrorOr<Unit>>
{
    #region Interfaces
    public async ValueTask<ErrorOr<Unit>> Handle(RemoveExpiredRefreshTokensCommand command, CancellationToken cancellationToken)
    {
        RefreshTokenEntity[] tokens = await thisDbContext.RefreshTokens.AsNoTracking().Where(e => DateTime.UtcNow > e.ExpiresAt).ToArrayAsync(cancellationToken);
        thisDbContext.RefreshTokens.RemoveRange(tokens);
        return Unit.Value;
    }
    #endregion
}
public sealed record class RemoveExpiredRefreshTokensCommand : IDbSaveMessage, ICommand<ErrorOr<Unit>>;