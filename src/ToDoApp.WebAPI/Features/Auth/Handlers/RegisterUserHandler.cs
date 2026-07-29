using Mediator;
using FluentResults;
using ToDoApp.WebAPI.Resources;
using ToDoApp.WebAPI.Extensions;
using Microsoft.AspNetCore.Identity;
using ToDoApp.Data.Features.Auth.Users;

namespace ToDoApp.WebAPI.Features.Auth.Handlers;

public sealed class RegisterUserHandler(
    UserManager<ApplicationUser> thisUserManager,
    ILogger<RegisterUserHandler> thisLogger
) : IRequestHandler<RegisterUserCommand, Result>
{
    #region Interfaces
    public async ValueTask<Result> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        if(await thisUserManager.FindByEmailAsync(request.Email) is not null)
        {
            string msg = string.Format(ErrorMessages.RecordExists, nameof(ApplicationUser), nameof(ApplicationUser.Email), request.Email);
            return Result.Fail(msg).LogTo(thisLogger).WithStatusCode(StatusCodes.Status409Conflict);
        }
        ApplicationUser user = new()
        {
            Email = request.Email,
            UserName = request.Email
        };
        IdentityResult result = await thisUserManager.CreateAsync(user);
        if(!result.Succeeded)
        {
            return result.ToFluentResult().LogTo(thisLogger);
        }
        result = await thisUserManager.AddPasswordAsync(user, request.Password);
        if(!result.Succeeded)
        {
            return result.ToFluentResult().LogTo(thisLogger);
        }
        return Result.Ok();
    }
    #endregion
}