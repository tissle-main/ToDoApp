using ErrorOr;
using Mediator;
using ToDoApp.Web.Shared.Behaviors.Authorized;

namespace ToDoApp.Web.Features.Tasks.Handlers.GetTasksByFilter;

public sealed record class GetTasksByFilterQuery(
    string? Search = null,
    string? Category = null,
    bool? Done = null,
    int? Skip = null,
    int? Take = null
) : IAuthorizedBehaviorMessage, IQuery<ErrorOr<GetTasksByFilterResponse>>;