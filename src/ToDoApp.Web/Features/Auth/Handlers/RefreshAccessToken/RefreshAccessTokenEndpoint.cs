using ErrorOr;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using ToDoApp.Web.Shared.Extensions;

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

    extension(IEndpointRouteBuilder thisBuilder)
    {
        public void AddRefreshAccessTokenEndpoint()
        {
            thisBuilder.MapPut(Url, RefreshAccessToken)
                .WithName(nameof(RefreshAccessToken))
                .Produces<RefreshAccessTokenResponse>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .ProducesProblem(StatusCodes.Status404NotFound);
        }
    }
    extension(HttpClient thisHttpClient)
    {
        public async ValueTask<HttpResponseMessage> SendRefreshAccessTokenAsync(RefreshAccessTokenCommand command, CancellationToken cancellationToken)
        {
            return await HttpClientJsonExtensions.PutAsJsonAsync(thisHttpClient, Url, command, cancellationToken);
        }
    }
}