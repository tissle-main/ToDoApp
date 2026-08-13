using Bogus;
using ErrorOr;
using Mediator;
using ToDoApp.Data;
using Microsoft.AspNetCore.Mvc;
using ToDoApp.Data.Features.Auth;
using ToDoApp.Web.Shared.Behaviors;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Web.Shared.Extensions;
using ToDoApp.Web.Features.Auth.Dtos;

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
            .FirstOrDefaultAsync(e => e.Value == command.RefreshToken, cancellationToken);
        if(entity is null)
        {
            return AuthErrors.RefreshTokenNotFound();
        }
        if(DateTime.UtcNow > entity.ExpiresAt)
        {
            return AuthErrors.RefreshTokenExpired();
        }
        thisDbContext.RefreshTokens.Remove(entity);
        ErrorOr<GenerateTokensResponse> errorOrTokens = await thisMediator.Send(new GenerateTokensCommand(entity.User!)
        {
            SaveDatabase = false
        }, cancellationToken);
        return errorOrTokens.Then(tokens =>
        {
            return new RefreshAccessTokenResponse(entity.User!.Email!, tokens.AccessToken, tokens.RefreshToken);
        });
    }
    #endregion
}
public sealed record class RefreshAccessTokenCommand(string RefreshToken) : IDbSaveMessage, ICommand<ErrorOr<RefreshAccessTokenResponse>>;
public sealed record class RefreshAccessTokenResponse(string Email, string AccessToken, RefreshTokenDto RefreshToken);
public static class RefreshAccessTokenCommandFaker
{
    public static Faker<RefreshAccessTokenCommand> ValidInstance(this Faker<RefreshAccessTokenCommand> faker)
    {
        return faker.CustomInstantiator(g =>
        {
            string refreshToken = new Faker<RefreshTokenEntity>().ValidInstance(default).Generate().Value;
            return new RefreshAccessTokenCommand(refreshToken);
        });
    }
}
public static class RefreshAccessTokenEndpoint
{
    public const string Url = "/auth/refresh-token";

    public static async Task<IResult> RefreshAccessToken(
        [FromBody] RefreshAccessTokenCommand command,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken
    )
    {
        ErrorOr<RefreshAccessTokenResponse> result = await mediator.Send(command, cancellationToken);
        return result.ToHttpResult();
    }
    public static void AddRefreshAccessTokenEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapPut(Url, RefreshAccessToken)
            .WithName(nameof(RefreshAccessToken))
            .Produces<RefreshAccessTokenResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
    public static async ValueTask<HttpResponseMessage> SendRefreshAccessTokenAsync(
        this HttpClient httpClient,
        RefreshAccessTokenCommand command,
        CancellationToken cancellationToken
    )
    {
        return await HttpClientJsonExtensions.PutAsJsonAsync(httpClient, Url, command, cancellationToken);
    }
}