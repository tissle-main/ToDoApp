using Mediator;
using ToDoApp.Data;
using FluentResults;
using ToDoApp.WebAPI.Resources;
using ToDoApp.WebAPI.Extensions;
using ToDoApp.Data.Features.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ToDoApp.WebAPI.Features.Tasks.Handlers;

public sealed class DeleteTasksHandler(
    AppDbContext thisDbContext,
    ILogger<DeleteTasksHandler> thisLogger
) : IRequestHandler<DeleteTasksCommand, Result>
{
    #region Interfaces
    public async ValueTask<Result> Handle(DeleteTasksCommand request, CancellationToken cancellationToken)
    {
        IQueryable<TaskEntity> query = thisDbContext.Tasks.AsNoTracking();
        if(request.Ids.Length == 0)
        {
            thisDbContext.RemoveRange(await query.ToArrayAsync(cancellationToken));
            await thisDbContext.SaveChangesAsync(cancellationToken);
            return Result.Ok();
        }
        TaskEntity[] entities = await query.Where(e => request.Ids.Contains(e.Id)).ToArrayAsync(cancellationToken);
        if(entities.Length != request.Ids.Length)
        {
            Guid[] missingIds = request.Ids.Except(entities.Select(dto => dto.Id)).ToArray();
            string idsString = string.Join(", ", missingIds);
            string msg = string.Format(ErrorMessages.RecordsNotFound, nameof(TaskEntity), nameof(TaskEntity.Id), idsString);
            return Result.Fail(msg).LogTo(thisLogger).WithStatusCode(StatusCodes.Status404NotFound);
        }
        thisDbContext.Tasks.RemoveRange(entities);
        await thisDbContext.SaveChangesAsync(cancellationToken);
        return Result.Ok();
    }
    #endregion
}