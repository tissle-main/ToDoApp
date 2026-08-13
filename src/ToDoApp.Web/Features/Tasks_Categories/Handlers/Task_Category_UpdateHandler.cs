using ErrorOr;
using Mediator;
using ToDoApp.Data;
using ToDoApp.Data.Features.Tasks;
using ToDoApp.Data.Features.Categories;
using ToDoApp.Data.Features.Tasks_Categories;
using ToDoApp.Web.Features.Categories;
using ToDoApp.Web.Features.Tasks;
using ToDoApp.Web.Shared.Handlers;

namespace ToDoApp.Web.Features.Tasks_Categories.Handlers;

public sealed class Task_Category_UpdateHandler(
    AppDbContext thisDbContext
) : UpdateJoinEntitiesHandler<Task_Category_UpdateCommand, Task_Category_JoinEntity, TaskEntity, CategoryEntity>(
        thisDbContext,
        TaskErrors.NotFound(),
        CategoryErrors.NotFound()
    ),
    ICommandHandler<Task_Category_UpdateCommand, ErrorOr<Unit>>;
public sealed record class Task_Category_UpdateCommand(
    IReadOnlyCollection<Task_Category_JoinEntity> OldEntities,
    IReadOnlyCollection<Task_Category_JoinEntity> NewEntities
) : IUpdateJoinEntitiesMessage<Task_Category_JoinEntity, TaskEntity, CategoryEntity>, ICommand<ErrorOr<Unit>>;