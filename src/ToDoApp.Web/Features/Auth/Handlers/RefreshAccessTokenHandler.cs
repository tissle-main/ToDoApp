using ErrorOr;
using Mediator;
using ToDoApp.Data;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Data.Features.Auth.RefreshTokens;

namespace ToDoApp.Web.Features.Auth.Handlers;

public sealed class RefreshAccessTokenHandler(
    AppDbContext thisDbContext,
    IMediator thisMediator
) : ICommandHandler<RefreshAccessTokenCommand, ErrorOr<RefreshAccessTokenResponse>>
{
    #region Interfaces
    public async ValueTask<ErrorOr<RefreshAccessTokenResponse>> Handle(RefreshAccessTokenCommand command, CancellationToken cancellationToken)
    {
        RefreshTokenEntity? entity = await thisDbContext.RefreshTokens
            .AsNoTracking()
            .Include(e => e.User)
            .FirstOrDefaultAsync(e => e.RefreshToken == command.RefreshToken, cancellationToken);
        if(entity is null)
        {
            return AuthErrors.RefreshTokenNotFound();
        }
        if(DateTime.UtcNow > entity.ExpiresAt)
        {
            return AuthErrors.RefreshTokenExpired();
        }
        thisDbContext.RefreshTokens.Remove(entity);
        await thisDbContext.SaveChangesAsync(cancellationToken);
        ErrorOr<GenerateTokensResponse> errorOrTokens = await thisMediator.Send(new GenerateTokensCommand(entity.User!), cancellationToken);
        return errorOrTokens.Then(tokens =>
        {
            return new RefreshAccessTokenResponse(entity.User!.Email!, tokens.AccessToken, tokens.RefreshToken);
        });
    }
    #endregion
}