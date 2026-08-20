using ErrorOr;
using Mediator;
using ToDoApp.Data;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Web.Shared.Extensions;
using ToDoApp.Web.Features.Auth.Dtos.Users;
using ToDoApp.Data.Features.Auth.RefreshTokens;
using ToDoApp.Web.Shared.Behaviors.DbTransaction;
using ToDoApp.Web.Features.Auth.Handlers.GenerateTokens;

namespace ToDoApp.Web.Features.Auth.Handlers.RefreshAccessToken;

public sealed class RefreshAccessTokenHandler(
    AppDbContext thisDbContext,
    IHttpContextAccessor thisHttpContextAccessor,
    IMediator thisMediator
) : ICommandHandler<RefreshAccessTokenCommand, ErrorOr<RefreshAccessTokenResponse>>
{
    #region Interfaces
    public async ValueTask<ErrorOr<RefreshAccessTokenResponse>> Handle(RefreshAccessTokenCommand command, CancellationToken cancellationToken)
    {
        if(thisHttpContextAccessor.HttpContext!.GetRefreshToken() is not string refreshToken)
        {
            return Error.Unauthorized();
        }
        RefreshTokenEntity? entity = await thisDbContext.RefreshTokens.Include(e => e.User).FirstOrDefaultAsync(
            e => e.Value == refreshToken,
            cancellationToken
        );
        if(entity is null || DateTime.UtcNow > entity.ExpiresAt)
        {
            return Error.Unauthorized();
        }
        thisDbContext.RefreshTokens.Remove(entity);
        await thisDbContext.SaveChangesAsync(cancellationToken);
        ErrorOr<GenerateTokensResponse> errorOrTokens = await thisMediator.Send(new GenerateTokensCommand(entity.User!)
        {
            BeginDbTransaction = false
        }, cancellationToken);
        return errorOrTokens.Then(tokens =>
        {
            thisHttpContextAccessor.HttpContext!.AddRefreshToken(tokens.RefreshToken);
            return new RefreshAccessTokenResponse(entity.User!.ToDto(), tokens.AccessToken);
        });
    }
    #endregion
}