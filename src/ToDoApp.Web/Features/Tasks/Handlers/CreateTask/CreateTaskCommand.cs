using ErrorOr;
using Mediator;
using ToDoApp.Web.Features.Tasks.Dtos;
using ToDoApp.Web.Shared.Behaviors.Authorized;
using ToDoApp.Web.Shared.Behaviors.DbTransaction;

namespace ToDoApp.Web.Features.Tasks.Handlers.CreateTask;

public sealed record class CreateTaskCommand(TaskDto Task) : IDbTransactionBehaviorMessage, IAuthorizedBehaviorMessage, ICommand<ErrorOr<CreateTaskResponse>>;