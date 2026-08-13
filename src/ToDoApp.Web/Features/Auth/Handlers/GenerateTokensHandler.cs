using ErrorOr;
using Mediator;
using ToDoApp.Data;
using ToDoApp.Data.Features.Auth;
using ToDoApp.Web.Shared.Behaviors;
using Microsoft.Extensions.Options;
using ToDoApp.Web.Features.Auth.Dtos;
using ToDoApp.Web.Features.Auth.Options;
using ToDoApp.Web.Features.Auth.Services;

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
        ErrorOr<Unit> result = await thisMediator.Send(new RemoveExpiredRefreshTokensCommand()
        {
            SaveDatabase = false
        }, cancellationToken);
        if(result.IsError)
        {
            return result.Errors;
        }

        string refreshToken = await thisRefreshTokenGenerator.GenerateTokenAsync(RefreshTokenEntityConstants.RefreshTokenMaxLength, cancellationToken);
        RefreshTokenEntity entity = new()
        {
            Value = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(tokenOptions.Value.ExpireDays),
            UserId = command.User.Id,
        };
        await thisDbContext.RefreshTokens.AddAsync(entity, cancellationToken);
        string accessToken = await thisAccessTokenGenerator.GenerateTokenAsync(command.User, cancellationToken);
        return new GenerateTokensResponse(accessToken, entity.ToDto());
    }
    #endregion
}
public sealed record class GenerateTokensCommand(UserEntity User) : IDbSaveMessage, ICommand<ErrorOr<GenerateTokensResponse>>;
public sealed record class GenerateTokensResponse(string AccessToken, RefreshTokenDto RefreshToken);