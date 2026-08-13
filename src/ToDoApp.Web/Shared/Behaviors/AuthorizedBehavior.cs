using ErrorOr;
using Mediator;
using ToDoApp.Web.Features;
using System.Security.Claims;
using ToDoApp.Data.Features.Auth;
using Microsoft.AspNetCore.Identity;
using System.Runtime.CompilerServices;

namespace ToDoApp.Web.Shared.Behaviors;

public sealed class AuthorizedBehavior<TMessage, TErrorOrValue>(
    UserManager<UserEntity> thisUserManager,
    IHttpContextAccessor thisHttpContextAccessor
) : BehaviorBase<TMessage, TErrorOrValue>
    where TMessage : IAuthorizedMessage
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
            return FromErrors([Error.Unauthorized()]);
        }
        message.User = user;
        return await next(message, cancellationToken);
    }
    #endregion
}
public static class AuthorizedBehaviorExtensions
{
    private static ConditionalWeakTable<IAuthorizedMessage, AuthorizedMetadata> MetadataTable { get; } = [];

    private static AuthorizedMetadata GetMetadata(IAuthorizedMessage message)
    {
        return MetadataTable.GetOrAdd(message, static _ => new AuthorizedMetadata());
    }

    extension(IAuthorizedMessage thisMessage)
    {
        public UserEntity User
        {
            get => GetMetadata(thisMessage).User;
            set
            {
                GetMetadata(thisMessage).User = value;
            }
        }
    }
}
public sealed class AuthorizedMetadata
{
    public UserEntity User { get; set; } = null!;
}
public interface IAuthorizedMessage : IMessage;
public sealed class AuthorizedFeatureProvider : FeatureProvider
{
    #region Base
    public override void AddServices(WebApplicationBuilder builder)
    {
        builder.Services.AddHttpContextAccessor();
    }
    #endregion
}