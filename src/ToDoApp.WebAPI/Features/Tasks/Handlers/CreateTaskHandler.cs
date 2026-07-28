using Mediator;
using ToDoApp.Data;
using FluentResults;
using ToDoApp.Data.Features.Tasks;
using ToDoApp.WebAPI.Features.Tasks.Dtos;
using ToDoApp.Data.Features.Tasks_Categories;

namespace ToDoApp.WebAPI.Features.Tasks.Handlers;

public sealed class CreateTaskHandler(
    AppDbContext thisDbContext,
    IJoinHandler<Task_Category_JoinEntity> thisJoinHandler
) : IRequestHandler<CreateTaskCommand, Result<Guid>>
{
    #region Interfaces
    public async ValueTask<Result<Guid>> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        TaskEntity entity = request.Task.ToEntity();
        entity.Id = Guid.Empty;

        await thisDbContext.AddAsync(entity, cancellationToken);
        Result result = await thisJoinHandler.Handle([], entity.Categories, cancellationToken);
        if(result.IsFailed)
        {
            return result.ToResult<Guid>();
        }
        await thisDbContext.SaveChangesAsync(cancellationToken);
        return Result.Ok(entity.Id);
    }
    #endregion
}