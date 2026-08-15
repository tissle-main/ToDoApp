using ToDoApp.Data.Features.Auth.Users;

namespace ToDoApp.Web.Shared.Behaviors.Authorized;

public sealed class AuthorizedBehaviorMessageExtraProperties
{
    public UserEntity User { get; set; } = null!;
}