using ErrorOr;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using ToDoApp.Web.Shared.Extensions;

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

    extension(IEndpointRouteBuilder thisBuilder)
    {
        public void AddLoginUserEndpoint()
        {
            thisBuilder.MapPost(Url, LoginUser)
                .WithName(nameof(LoginUser))
                .Produces<LoginUserResponse>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status404NotFound)
                .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity);
        }
    }
    extension(HttpClient thisHttpClient)
    {
        public async ValueTask<HttpResponseMessage> SendLoginUserAsync(LoginUserCommand command, CancellationToken cancellationToken)
        {
            return await HttpClientJsonExtensions.PostAsJsonAsync(thisHttpClient, Url, command, cancellationToken);
        }
    } 
}