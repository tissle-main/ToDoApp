using Mediator;
using FluentResults;
using ToDoApp.WebAPI.Extensions;
using Microsoft.AspNetCore.Identity;
using ToDoApp.Data.Features.Auth.Users;

namespace ToDoApp.WebAPI.Features.Auth.Handlers;

public sealed class DeleteUserHandler(
    UserManager<ApplicationUser> thisUserManager,
    ILogger<DeleteUserHandler> thisLogger,
    IHttpContextAccessor thisHttpContext
) : IRequestHandler<DeleteUserCommand, Result>
{
    #region Interfaces
    public async ValueTask<Result> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        if(await thisUserManager.GetUserAsync(thisHttpContext.HttpContext!.User) is not ApplicationUser user)
        {
            return Result.Fail("").WithStatusCode(StatusCodes.Status401Unauthorized).LogTo(thisLogger);
        }
        IdentityResult result = await thisUserManager.DeleteAsync(user);
        return result.ToFluentResult();
    }
    #endregion
}