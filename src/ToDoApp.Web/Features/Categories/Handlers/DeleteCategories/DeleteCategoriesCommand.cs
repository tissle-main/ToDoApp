using ErrorOr;
using Mediator;
using ToDoApp.Web.Shared.Behaviors.Authorized;
using ToDoApp.Web.Shared.Behaviors.DbTransaction;

namespace ToDoApp.Web.Features.Categories.Handlers.DeleteCategories;

public sealed record class DeleteCategoriesCommand(Guid[] Ids) : IDbTransactionBehaviorMessage, IAuthorizedBehaviorMessage, ICommand<ErrorOr<Unit>>;