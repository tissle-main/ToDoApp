using ErrorOr;
using Mediator;
using ToDoApp.Web.Features.Tasks.Dtos;
using ToDoApp.Web.Shared.Behaviors.Authorized;
using ToDoApp.Web.Shared.Behaviors.DbTransaction;

namespace ToDoApp.Web.Features.Tasks.Handlers.UpdateTask;

public sealed record class UpdateTaskCommand(TaskDto Task) : IDbTransactionBehaviorMessage, IAuthorizedBehaviorMessage, ICommand<ErrorOr<Unit>>;