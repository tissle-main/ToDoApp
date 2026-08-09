using ErrorOr;
using Mediator;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using ToDoApp.Data.Features.Auth.Users;

namespace ToDoApp.Web.Shared.Behaviors.Authorized;

public sealed class AuthorizedMessageBehavior<TMessage, TErrorOrValue>(
    UserManager<UserEntity> thisUserManager,
    IHttpContextAccessor thisHttpContextAccessor
) : BehaviorBase<TMessage, TErrorOrValue>
    where TMessage : AuthorizedMessage
    where TErrorOrValue : IErrorOr
{
    #region Base
    public override async ValueTask<TErrorOrValue> Handle(
        TMessage message,
        MessageHandlerDelegate<TMessage, TErrorOrValue> next,
        CancellationToken cancellationToken
    )
    {
        ClaimsPrincipal principal = thisHttpContextAccessor.HttpContext!.User;
        if(await thisUserManager.GetUserAsync(principal) is not UserEntity user)
        {
            return FromErrors([GeneralErrors.Unauthorized()]);
        }
        return await next(message with { User = user }, cancellationToken);
    }
    #endregion
}