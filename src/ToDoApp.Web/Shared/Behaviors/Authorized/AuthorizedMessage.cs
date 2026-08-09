using Mediator;
using ToDoApp.Data.Features.Auth.Users;

namespace ToDoApp.Web.Shared.Behaviors.Authorized;

public abstract record class AuthorizedMessage(UserEntity User = null!) : IMessage;