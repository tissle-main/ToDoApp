using ErrorOr;
using Mediator;
using ToDoApp.Data;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Web.Features.Auth.Options;
using ToDoApp.Web.Features.Auth.Services;
using ToDoApp.Data.Features.Auth.RefreshTokens;

namespace ToDoApp.Web.Features.Auth.Handlers;

public sealed class GenerateTokensHandler(
    AppDbContext thisDbContext,
    IMediator thisMediator,
    IOptionsSnapshot<RefreshTokenOptions> tokenOptions,
    IAccessTokenGenerator thisAccessTokenGenerator,
    IRefreshTokenGenerator thisRefreshTokenGenerator
) : ICommandHandler<GenerateTokensCommand, ErrorOr<GenerateTokensResponse>>
{
    #region Interfaces
    public async ValueTask<ErrorOr<GenerateTokensResponse>> Handle(GenerateTokensCommand command, CancellationToken cancellationToken)
    {
        ErrorOr<Unit> result = await thisMediator.Send(new RemoveRefreshTokensCommand(command.User), cancellationToken);
        if(result.IsError)
        {
            return result.Errors;
        }

        string refreshToken;
        do
        {
            refreshToken = await thisRefreshTokenGenerator.GenerateTokenAsync(RefreshTokenEntityConstants.RefreshTokenMaxLength / 2, cancellationToken);
        }
        while(await thisDbContext.RefreshTokens.AsNoTracking().FirstOrDefaultAsync(e => e.RefreshToken == refreshToken, cancellationToken) is not null);
        RefreshTokenEntity entity = new()
        {
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(tokenOptions.Value.RefreshTokenDurationInDays),
            UserId = command.User.Id,
        };
        await thisDbContext.RefreshTokens.AddAsync(entity, cancellationToken);
        await thisDbContext.SaveChangesAsync(cancellationToken);
        string accessToken = await thisAccessTokenGenerator.GenerateTokenAsync(command.User, cancellationToken);
        return new GenerateTokensResponse(accessToken, refreshToken);
    }
    #endregion
}