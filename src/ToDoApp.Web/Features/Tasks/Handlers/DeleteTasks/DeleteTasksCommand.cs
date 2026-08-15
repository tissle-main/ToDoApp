using ErrorOr;
using Mediator;
using ToDoApp.Web.Shared.Behaviors.Authorized;
using ToDoApp.Web.Shared.Behaviors.DbTransaction;

namespace ToDoApp.Web.Features.Tasks.Handlers.DeleteTasks;

public sealed record class DeleteTasksCommand(Guid[] Ids) : IDbTransactionBehaviorMessage, IAuthorizedBehaviorMessage, ICommand<ErrorOr<Unit>>;