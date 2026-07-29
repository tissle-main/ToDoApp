using Mediator;
using FluentResults;
using ToDoApp.WebAPI.Resources;
using ToDoApp.WebAPI.Extensions;
using ToDoApp.WebAPI.Services.Jwt;
using Microsoft.AspNetCore.Identity;
using ToDoApp.Data.Features.Auth.Users;

namespace ToDoApp.WebAPI.Features.Auth.Handlers;

public sealed class LoginUserHandler(
    SignInManager<ApplicationUser> thisSignInManager,
    ILogger<RegisterUserHandler> thisLogger,
    IJwtService thisJwtService
) : IRequestHandler<LoginUserCommand, Result<string>>
{
    #region Interfaces
    public async ValueTask<Result<string>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        if(await thisSignInManager.UserManager.FindByEmailAsync(request.Email) is not ApplicationUser user)
        {
            string msg = string.Format(ErrorMessages.RecordNotFound, nameof(ApplicationUser), nameof(ApplicationUser.Email), request.Email);
            return Result.Fail(msg).LogTo(thisLogger).WithStatusCode(StatusCodes.Status404NotFound);
        }
        SignInResult result = await thisSignInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: false);
        if(!result.Succeeded)
        {
            return result.ToFluentResult(string.Empty)!;
        }
        return await thisJwtService.GenerateTokenAsync(user);
    }
    #endregion
}