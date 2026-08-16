using ErrorOr;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using ToDoApp.Web.Shared.Extensions;
using ToDoApp.Web.Features.Auth.Handlers.GenerateTokens;

namespace ToDoApp.Web.Features.Auth.Handlers.RefreshAccessToken;

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

    extension(RouteHandlerBuilder thisBuilder)
    {
        public RouteHandlerBuilder AddRefreshAccessTokenProductionProblems()
        {
            thisBuilder.ProducesProblem(StatusCodes.Status400BadRequest);
            thisBuilder.ProducesProblem(StatusCodes.Status404NotFound);
            return thisBuilder.AddGenerateTokensProductionProblems();
        }
    }
    extension(IEndpointRouteBuilder thisBuilder)
    {
        public void AddRefreshAccessTokenEndpoint()
        {
            thisBuilder.MapPut(Url, RefreshAccessToken)
                .WithName(nameof(RefreshAccessToken))
                .Produces<RefreshAccessTokenResponse>(StatusCodes.Status200OK)
                .AddRefreshAccessTokenProductionProblems();
        }
    }
    extension(HttpClient thisHttpClient)
    {
        public async ValueTask<HttpResponseMessage> SendRefreshAccessTokenAsync(RefreshAccessTokenCommand command, CancellationToken cancellationToken)
        {
            return await thisHttpClient.PutAsJsonAsync(Url, command, cancellationToken);
        }
    }
}