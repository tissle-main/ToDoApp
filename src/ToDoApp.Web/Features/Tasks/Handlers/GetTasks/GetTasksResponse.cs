using ErrorOr;
using Mediator;
using ToDoApp.Web.Shared.Behaviors.Authorized;

namespace ToDoApp.Web.Features.Tasks.Handlers.GetTasks;

public sealed record class GetTasksQuery(Guid[] Ids) : IAuthorizedBehaviorMessage, IQuery<ErrorOr<GetTasksResponse>>;