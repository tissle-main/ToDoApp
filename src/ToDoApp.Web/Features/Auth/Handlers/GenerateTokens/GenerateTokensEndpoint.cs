using ErrorOr;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using ToDoApp.Web.Shared.Extensions;
using ToDoApp.Data.Features.Auth.Users;
using ToDoApp.Web.Features.Auth.Handlers.RemoveExpiredRefreshTokens;

namespace ToDoApp.Web.Features.Auth.Handlers.GenerateTokens;

public static class GenerateTokensEndpoint
{
    public const string Url = "/api/auth/generate-tokens";

    public static async Task<IResult> GenerateTokens(
        [FromBody] UserEntity user,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken
    )
    {
        ErrorOr<GenerateTokensResponse> errorOrUnit = await mediator.Send(new GenerateTokensCommand(user), cancellationToken);
        return errorOrUnit.ToHttpResult();
    }

    extension(RouteHandlerBuilder thisBuilder)
    {
        public RouteHandlerBuilder AddGenerateTokensProductionProblems()
        {
            return thisBuilder.AddRemoveExpiredRefreshTokensProductionProblems();
        }
    }
    extension(IEndpointRouteBuilder thisBuilder)
    {
        public void AddGenerateTokensEndpoint()
        {
            thisBuilder.MapPost(Url, GenerateTokens)
                .WithName(nameof(GenerateTokens))
                .Produces<GenerateTokensResponse>(StatusCodes.Status200OK)
                .AddGenerateTokensProductionProblems();
        }
    }
    extension(HttpClient thisHttpClient)
    {
        public async ValueTask<HttpResponseMessage> SendGenerateTokensAsync(UserEntity user, CancellationToken cancellationToken)
        {
            return await thisHttpClient.PostAsJsonAsync(Url, user, cancellationToken);
        }
    }
}