using Mediator;
using ToDoApp.Data;
using FluentResults;
using ToDoApp.WebAPI.Resources;
using ToDoApp.WebAPI.Extensions;
using ToDoApp.Data.Features.Tasks;
using Microsoft.EntityFrameworkCore;
using ToDoApp.WebAPI.Features.Tasks.Dtos;

namespace ToDoApp.WebAPI.Features.Tasks.Handlers;

public sealed class GetTaskByIdHandler(
    AppDbContext thisDbContext,
    ILogger<GetTaskByIdHandler> thisLogger
) : IRequestHandler<GetTaskByIdQuery, Result<TaskDto>>
{
    #region Interfaces
    public async ValueTask<Result<TaskDto>> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
    {
        TaskEntity? entity = await thisDbContext.Tasks.AsNoTracking()
            .Include(e => e.Categories).ThenInclude(je => je.Category)
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);
        if(entity is null)
        {
            string msg = string.Format(ErrorMessages.RecordNotFound, nameof(TaskEntity), nameof(TaskEntity.Id), request.Id);
            return Result.Fail(msg).LogTo(thisLogger).WithStatusCode(StatusCodes.Status404NotFound);
        }
        return Result.Ok(entity.ToDto());
    }
    #endregion
}