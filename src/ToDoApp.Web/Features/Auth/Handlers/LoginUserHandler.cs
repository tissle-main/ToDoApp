using ErrorOr;
using Mediator;
using Microsoft.AspNetCore.Identity;
using ToDoApp.Data.Features.Auth.Users;

namespace ToDoApp.Web.Features.Auth.Handlers;

public sealed class LoginUserHandler(
    SignInManager<UserEntity> thisSignInManager,
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
        SignInResult result = await thisSignInManager.CheckPasswordSignInAsync(user, command.Password, lockoutOnFailure: false);
        if(!result.Succeeded)
        {
            return result.ToError();
        }
        ErrorOr<GenerateTokensResponse> errorOrTokens = await thisMediator.Send(new GenerateTokensCommand(user), cancellationToken);
        return errorOrTokens.Then(tokens =>
        {
            return new LoginUserResponse(user.Email!, tokens.AccessToken, tokens.RefreshToken);
        });    
    }
    #endregion
}