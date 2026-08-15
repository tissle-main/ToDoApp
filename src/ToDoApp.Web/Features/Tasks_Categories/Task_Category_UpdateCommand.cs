using ErrorOr;
using Mediator;
using ToDoApp.Data.Features.Tasks;
using ToDoApp.Web.Shared.JoinEntities;
using ToDoApp.Data.Features.Categories;
using ToDoApp.Data.Features.Tasks_Categories;

namespace ToDoApp.Web.Features.Tasks_Categories;

public sealed record class Task_Category_UpdateCommand(
    IReadOnlyCollection<Task_Category_JoinEntity> OldEntities,
    IReadOnlyCollection<Task_Category_JoinEntity> NewEntities
) : IUpdateJoinEntitiesMessage<Task_Category_JoinEntity, TaskEntity, CategoryEntity>, ICommand<ErrorOr<Unit>>;