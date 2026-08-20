using ErrorOr;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using ToDoApp.Web.Shared.Extensions;

namespace ToDoApp.Web.Features.Auth.Handlers.RemoveExpiredRefreshTokens;

public static class RemoveExpiredRefreshTokensEndpoint
{
    public const string Url = "/api/auth/remove-expired-refresh-tokens";

    public static async Task<IResult> RemoveExpiredRefreshTokens(
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken
    )
    {
        ErrorOr<Unit> errorOrUnit = await mediator.Send(new RemoveExpiredRefreshTokensCommand(), cancellationToken);
        return errorOrUnit.ToHttpResult();
    }

    extension(RouteHandlerBuilder thisBuilder)
    {
        public RouteHandlerBuilder AddRemoveExpiredRefreshTokensProductionProblems()
        {
            return thisBuilder;
        }
    }
    extension(IEndpointRouteBuilder thisBuilder)
    {
        public void AddRemoveExpiredRefreshTokensEndpoint()
        {
            thisBuilder.MapPost(Url, RemoveExpiredRefreshTokens)
                .WithName(nameof(RemoveExpiredRefreshTokens))
                .Produces(StatusCodes.Status204NoContent)
                .AddRemoveExpiredRefreshTokensProductionProblems();
        }
    }
    extension(HttpClient thisHttpClient)
    {
        public async ValueTask<HttpResponseMessage> SendRemoveExpiredRefreshTokensAsync(CancellationToken cancellationToken)
        {
            return await thisHttpClient.PostAsync(Url, null, cancellationToken);
        }
    }
}