using ErrorOr;
using Mediator;
using ToDoApp.Data;
using ToDoApp.Data.Features.Categories;
using ToDoApp.Web.Features.Categories.Dtos;
using ToDoApp.Web.Features.Tasks_Categories;
using ToDoApp.Data.Features.Tasks_Categories;
using ToDoApp.Web.Shared.Behaviors.Authorized;
using ToDoApp.Web.Shared.Behaviors.DbTransaction;

namespace ToDoApp.Web.Features.Categories.Handlers.CreateCategory;

public sealed class CreateCategoryHandler(
    AppDbContext thisDbContext,
    IMediator thisMediator
) : ICommandHandler<CreateCategoryCommand, ErrorOr<CreateCategoryResponse>>
{
    #region Interfaces
    public async ValueTask<ErrorOr<CreateCategoryResponse>> Handle(CreateCategoryCommand command, CancellationToken cancellationToken)
    {
        CategoryEntity entity = command.Category.ToEntity();
        List<Task_Category_JoinEntity> newEntities = entity.Tasks;
        entity.UserId = command.User.Id;
        entity.Tasks = [];

        await thisDbContext.Categories.AddAsync(entity, cancellationToken);
        await thisDbContext.SaveChangesAsync(cancellationToken);

        foreach(Task_Category_JoinEntity je in newEntities)
        {
            je.RightId = entity.Id;
        }
        ErrorOr<Unit> errorOrValue = await thisMediator.Send(new Task_Category_UpdateCommand([], newEntities)
        {
            BeginDbTransaction = false
        }, cancellationToken);
        return errorOrValue.Then(value => new CreateCategoryResponse(entity.Id));
    }
    #endregion
}