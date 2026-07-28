using Mediator;
using ToDoApp.Data;
using FluentResults;
using ToDoApp.WebAPI.Resources;
using ToDoApp.WebAPI.Extensions;
using ToDoApp.Data.Features.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ToDoApp.WebAPI.Features.Tasks.Handlers;

public sealed class DeleteTaskHandler(
    AppDbContext thisDbContext,
    ILogger<DeleteTaskHandler> thisLogger
) : IRequestHandler<DeleteTaskCommand, Result>
{
    #region Interfaces
    public async ValueTask<Result> Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
    {
        TaskEntity? entity = await thisDbContext.Tasks.AsNoTracking().FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);
        if(entity is null)
        {
            string msg = string.Format(ErrorMessages.RecordNotFound, nameof(TaskEntity), nameof(TaskEntity.Id), request.Id);
            return Result.Fail(msg).LogTo(thisLogger).WithStatusCode(StatusCodes.Status404NotFound);
        }
        thisDbContext.Remove(entity);
        await thisDbContext.SaveChangesAsync(cancellationToken);
        return Result.Ok();
    }
    #endregion
}