using ErrorOr;
using Mediator;
using ToDoApp.Web.Shared.Behaviors.DbTransaction;

namespace ToDoApp.Web.Features.Auth.Handlers.LoginUser;

public sealed record class LoginUserCommand(string Email, string Password) : IDbTransactionBehaviorMessage, ICommand<ErrorOr<LoginUserResponse>>;