using ErrorOr;
using Mediator;
using ToDoApp.Web.Shared.Behaviors.Authorized;

namespace ToDoApp.Web.Features.Auth.Handlers;

public sealed record class DeleteUserCommand : AuthorizedMessage, ICommand<ErrorOr<Unit>>;