using ErrorOr;
using Mediator;
using ToDoApp.Data;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Data.Features.Auth.RefreshTokens;

namespace ToDoApp.Web.Features.Auth.Handlers.RemoveExpiredRefreshTokens;

public sealed class RemoveExpiredRefreshTokensHandler(AppDbContext thisDbContext) : ICommandHandler<RemoveExpiredRefreshTokensCommand, ErrorOr<Unit>>
{
    #region Interfaces
    public async ValueTask<ErrorOr<Unit>> Handle(RemoveExpiredRefreshTokensCommand command, CancellationToken cancellationToken)
    {
        RefreshTokenEntity[] tokens = await thisDbContext.RefreshTokens.Where(e => DateTime.UtcNow > e.ExpiresAt).ToArrayAsync(cancellationToken);
        thisDbContext.RefreshTokens.RemoveRange(tokens);
        await thisDbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
    #endregion
}