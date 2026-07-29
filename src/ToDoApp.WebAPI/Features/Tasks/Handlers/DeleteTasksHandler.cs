using FluentResults;
using Mediator;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Data;
using ToDoApp.Data.Features.Auth.Users;
using ToDoApp.Data.Features.Tasks;
using ToDoApp.Data.Features.Tasks_Categories;
using ToDoApp.WebAPI.Extensions;
using ToDoApp.WebAPI.Resources;

namespace ToDoApp.WebAPI.Features.Tasks.Handlers;

public sealed class DeleteTasksHandler(
    AppDbContext thisDbContext,
    ILogger<DeleteTasksHandler> thisLogger,
    IJoinHandler<Task_Category_JoinEntity> thisJoinHandler,
    IHttpContextAccessor thisHttpContext,
    UserManager<ApplicationUser> thisUserManager
) : IRequestHandler<DeleteTasksCommand, Result>
{
    #region Interfaces
    public async ValueTask<Result> Handle(DeleteTasksCommand request, CancellationToken cancellationToken)
    {
        if(await thisUserManager.GetUserAsync(thisHttpContext.HttpContext!.User) is not ApplicationUser user)
        {
            return Result.Fail("").WithStatusCode(StatusCodes.Status401Unauthorized).LogTo(thisLogger);
        }
        IQueryable<TaskEntity> query = thisDbContext.Tasks.AsNoTracking().Where(e => e.UserId == user.Id);
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
        Guid[] ids = entities.Select(e => e.Id).ToArray();
        List<Task_Category_JoinEntity> joinEntities = await thisDbContext.Tasks_Categories.Where(je => ids.Contains(je.TaskId)).ToListAsync(cancellationToken);
        Result result = await thisJoinHandler.Handle(joinEntities, [], cancellationToken);
        if(result.IsFailed)
        {
            return result;
        }
        thisDbContext.Tasks.RemoveRange(entities);
        await thisDbContext.SaveChangesAsync(cancellationToken);
        return Result.Ok();
    }
    #endregion
}