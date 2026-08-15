using ErrorOr;
using Mediator;
using ToDoApp.Data;
using ToDoApp.Data.Features.Tasks;
using ToDoApp.Web.Shared.JoinEntities;
using ToDoApp.Data.Features.Categories;
using ToDoApp.Data.Features.Tasks_Categories;

namespace ToDoApp.Web.Features.Tasks_Categories;

public sealed class Task_Category_UpdateHandler(
    AppDbContext thisDbContext
) : UpdateJoinEntitiesHandler<Task_Category_UpdateCommand, Task_Category_JoinEntity, TaskEntity, CategoryEntity>(thisDbContext),
    ICommandHandler<Task_Category_UpdateCommand, ErrorOr<Unit>>;