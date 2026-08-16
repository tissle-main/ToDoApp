using ErrorOr;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using ToDoApp.Web.Shared.Extensions;
using ToDoApp.Web.Features.Auth.Handlers.GenerateTokens;

namespace ToDoApp.Web.Features.Auth.Handlers.LoginUser;

public static class LoginUserEndpoint
{
    public const string Url = "/auth/login";

    public static async Task<IResult> LoginUser(
        [FromBody] LoginUserCommand command,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken
    )
    {
        ErrorOr<LoginUserResponse> result = await mediator.Send(command, cancellationToken);
        return result.ToHttpResult();
    }

    extension(RouteHandlerBuilder thisBuilder)
    {
        public RouteHandlerBuilder AddLoginUserProductionProblems()
        {
            thisBuilder.ProducesProblem(StatusCodes.Status400BadRequest);
            thisBuilder.ProducesProblem(StatusCodes.Status404NotFound);
            thisBuilder.ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity);
            return thisBuilder.AddGenerateTokensProductionProblems();
        }
    }
    extension(IEndpointRouteBuilder thisBuilder)
    {
        public void AddLoginUserEndpoint()
        {
            thisBuilder.MapPost(Url, LoginUser)
                .WithName(nameof(LoginUser))
                .Produces<LoginUserResponse>(StatusCodes.Status200OK)
                .AddLoginUserProductionProblems();
        }
    }
    extension(HttpClient thisHttpClient)
    {
        public async ValueTask<HttpResponseMessage> SendLoginUserAsync(LoginUserCommand command, CancellationToken cancellationToken)
        {
            return await thisHttpClient.PostAsJsonAsync(Url, command, cancellationToken);
        }
    } 
}