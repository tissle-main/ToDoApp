using ErrorOr;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using ToDoApp.Web.Shared.Extensions;

namespace ToDoApp.Web.Features.Auth.Handlers.RegisterUser;

public static class RegisterUserEndpoint
{
    public const string Url = "/auth/register";

    public static async Task<IResult> RegisterUser(
        [FromBody] RegisterUserCommand command,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken
    )
    {
        ErrorOr<Unit> errorOrUnit = await mediator.Send(command, cancellationToken);
        return errorOrUnit.ToHttpResult();
    }

    extension(IEndpointRouteBuilder thisBuilder)
    {
        public void AddRegisterUserEndpoint()
        {
            thisBuilder.MapPost(Url, RegisterUser)
                .WithName(nameof(RegisterUser))
                .Produces(StatusCodes.Status204NoContent)
                .ProducesProblem(StatusCodes.Status409Conflict)
                .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity);
        }
    }
    extension(HttpClient thisHttpClient)
    {
        public async ValueTask<HttpResponseMessage> SendRegisterUserAsync(RegisterUserCommand command, CancellationToken cancellationToken)
        {
            return await HttpClientJsonExtensions.PostAsJsonAsync(thisHttpClient, Url, command, cancellationToken);
        }
    }
}