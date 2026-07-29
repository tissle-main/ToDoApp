using Mediator;
using FluentResults;
using Microsoft.AspNetCore.Mvc;
using ToDoApp.WebAPI.Extensions;
using Microsoft.AspNetCore.Authorization;
using ToDoApp.WebAPI.Features.Auth.Handlers;

namespace ToDoApp.WebAPI.Features.Auth;

[ApiController]
[Route("/api/auth")]
public sealed class AuthController : ControllerBase
{
    [HttpPost("/register", Name = nameof(RegisterUser))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<HttpValidationProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> RegisterUser(
        [FromBody] RegisterUserCommand request,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken
    )
    {
        Result result = await mediator.Send(request, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost("/login", Name = nameof(LoginUser))]
    [ProducesResponseType<string>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<HttpValidationProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> LoginUser(
        [FromBody] LoginUserCommand request,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken
    )
    {
        Result<string> result = await mediator.Send(request, cancellationToken);
        return result.ToActionResult();
    }

    [Authorize]
    [HttpDelete("/", Name = nameof(DeleteUser))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteUser(
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken
    )
    {
        Result result = await mediator.Send(new DeleteUserCommand(), cancellationToken);
        return result.ToActionResult();
    }
}