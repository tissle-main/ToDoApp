using ErrorOr;
using Mediator;
using ToDoApp.Web.Shared.Behaviors.Authorized;

namespace ToDoApp.Web.Features.Categories.Handlers.GetCategories;

public sealed record class GetCategoriesQuery(Guid[] Ids) : IAuthorizedBehaviorMessage, IQuery<ErrorOr<GetCategoriesResponse>>;