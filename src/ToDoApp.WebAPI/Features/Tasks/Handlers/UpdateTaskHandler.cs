using Mediator;
using ToDoApp.Data;
using FluentResults;
using ToDoApp.WebAPI.Resources;
using ToDoApp.WebAPI.Extensions;
using ToDoApp.Data.Features.Tasks;
using Microsoft.EntityFrameworkCore;
using ToDoApp.WebAPI.Features.Tasks.Dtos;
using ToDoApp.Data.Features.Tasks_Categories;

namespace ToDoApp.WebAPI.Features.Tasks.Handlers;

public sealed class UpdateTaskHandler(
    AppDbContext thisDbContext,
    ILogger<UpdateTaskHandler> thisLogger,
    IJoinHandler<Task_Category_JoinEntity> thisJoinHandler
) : IRequestHandler<UpdateTaskCommand, Result>
{
    #region Interfaces
    public async ValueTask<Result> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
    {
        TaskEntity newEntity = request.Task.ToEntity();
        TaskEntity? oldEntity = await thisDbContext.Tasks.AsNoTracking()
            .Include(e => e.Categories)
            .FirstOrDefaultAsync(e => e.Id == newEntity.Id, cancellationToken);
        if(oldEntity is null)
        {
            string msg = string.Format(ErrorMessages.RecordNotFound, nameof(TaskEntity), nameof(TaskEntity.Id), request.Task.Id);
            return Result.Fail(msg).LogTo(thisLogger).WithStatusCode(StatusCodes.Status404NotFound);
        }
        List<Task_Category_JoinEntity> oldList = oldEntity.Categories;
        List<Task_Category_JoinEntity> newList = newEntity.Categories;
        newEntity.MapToEntity(oldEntity);
        thisDbContext.Update(oldEntity);
        Result result = await thisJoinHandler.Handle(oldList, newList, cancellationToken);
        if(result.IsFailed)
        {
            return result;
        }
        await thisDbContext.SaveChangesAsync(cancellationToken);
        return Result.Ok();
    }
    #endregion
}