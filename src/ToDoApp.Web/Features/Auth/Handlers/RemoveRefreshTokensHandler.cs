using ErrorOr;
using Mediator;
using ToDoApp.Data;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Data.Features.Auth.RefreshTokens;

namespace ToDoApp.Web.Features.Auth.Handlers;

public sealed class RemoveRefreshTokensHandler(AppDbContext thisDbContext) : ICommandHandler<RemoveRefreshTokensCommand, ErrorOr<Unit>>
{
    #region Interfaces
    public async ValueTask<ErrorOr<Unit>> Handle(RemoveRefreshTokensCommand command, CancellationToken cancellationToken)
    {
        RefreshTokenEntity[] tokens = await thisDbContext.RefreshTokens.AsNoTracking().Where(
            e => e.UserId == command.User.Id && DateTime.UtcNow > e.ExpiresAt
        ).ToArrayAsync(cancellationToken);
        thisDbContext.RemoveRange(tokens);
        await thisDbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
    #endregion
}