using ErrorOr;
using Mediator;
using ToDoApp.Web.Shared.Extensions;
using Microsoft.AspNetCore.Identity;
using ToDoApp.Data.Features.Auth.Users;
using ToDoApp.Web.Shared.Behaviors.DbTransaction;
using ToDoApp.Web.Features.Auth.Handlers.GenerateTokens;
using LoginResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace ToDoApp.Web.Features.Auth.Handlers.LoginUser;

public sealed class LoginUserHandler(
    SignInManager<UserEntity> thisSignInManager,
    IHttpContextAccessor thisHttpContextAccessor,
    IMediator thisMediator
) : ICommandHandler<LoginUserCommand, ErrorOr<LoginUserResponse>>
{
    #region Interfaces
    public async ValueTask<ErrorOr<LoginUserResponse>> Handle(LoginUserCommand command, CancellationToken cancellationToken)
    {
        if(await thisSignInManager.UserManager.FindByEmailAsync(command.Email) is not UserEntity user)
        {
            return AuthErrors.UserNotFound();
        }
        LoginResult result = await thisSignInManager.CheckPasswordSignInAsync(user, command.Password, lockoutOnFailure: false);
        if(!result.Succeeded)
        {
            return result.ToError();
        }
        ErrorOr<GenerateTokensResponse> errorOrTokens = await thisMediator.Send(new GenerateTokensCommand(user)
        {
            BeginDbTransaction = false
        }, cancellationToken);
        return errorOrTokens.Then(tokens =>
        {
            thisHttpContextAccessor.HttpContext!.AddRefreshToken(tokens.RefreshToken);
            return new LoginUserResponse(user.Email!, tokens.AccessToken);
        });    
    }
    #endregion
}