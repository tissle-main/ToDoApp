using ErrorOr;
using Mediator;
using ToDoApp.Web.Shared.Behaviors.DbTransaction;

namespace ToDoApp.Web.Features.Auth.Handlers.RegisterUser;

public sealed record class RegisterUserCommand(string Email, string Password) : IDbTransactionBehaviorMessage, ICommand<ErrorOr<Unit>>;