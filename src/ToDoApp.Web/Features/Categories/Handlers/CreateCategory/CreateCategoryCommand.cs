using ErrorOr;
using Mediator;
using ToDoApp.Web.Features.Categories.Dtos;
using ToDoApp.Web.Shared.Behaviors.Authorized;
using ToDoApp.Web.Shared.Behaviors.DbTransaction;

namespace ToDoApp.Web.Features.Categories.Handlers.CreateCategory;

public sealed record class CreateCategoryCommand(
    CategoryDto Category
) : IDbTransactionBehaviorMessage,
    IAuthorizedBehaviorMessage,
    ICommand<ErrorOr<CreateCategoryResponse>>;