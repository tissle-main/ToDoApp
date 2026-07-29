using FluentResults;
using Mediator;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Data;
using ToDoApp.Data.Features.Auth.Users;
using ToDoApp.Data.Features.Tasks;
using ToDoApp.WebAPI.Extensions;
using ToDoApp.WebAPI.Features.Categories.Dtos;
using ToDoApp.WebAPI.Features.Tasks.Dtos;
using ToDoApp.WebAPI.Resources;

namespace ToDoApp.WebAPI.Features.Tasks.Handlers;

public sealed class GetTasksHandler(
    AppDbContext thisDbContext,
    ILogger<GetTasksHandler> thisLogger,
    IHttpContextAccessor thisHttpContext,
    UserManager<ApplicationUser> thisUserManager
) : IRequestHandler<GetTasksQuery, Result<TaskDto[]>>
{
    #region Interfaces
    public async ValueTask<Result<TaskDto[]>> Handle(GetTasksQuery request, CancellationToken cancellationToken)
    {
        if(await thisUserManager.GetUserAsync(thisHttpContext.HttpContext!.User) is not ApplicationUser user)
        {
            return Result.Fail("").WithStatusCode(StatusCodes.Status401Unauthorized).LogTo(thisLogger);
        }
        IQueryable<TaskEntity> query = thisDbContext.Tasks.AsNoTracking().Include(e => e.Categories).Where(t => t.UserId == user.Id);
        if(request.Ids.Length == 0)
        {
            return await query.ProjectToDtos().ToArrayAsync(cancellationToken);
        }
        TaskDto[] dtos = await query.Where(e => request.Ids.Contains(e.Id)).ProjectToDtos().ToArrayAsync(cancellationToken);
        if(dtos.Length != request.Ids.Length)
        {
            Guid[] missingIds = request.Ids.Except(dtos.Select(dto => dto.Id)).ToArray();
            string idsString = string.Join(", ", missingIds);
            string msg = string.Format(ErrorMessages.RecordsNotFound, nameof(TaskEntity), nameof(TaskEntity.Id), idsString);
            return Result.Fail(msg).LogTo(thisLogger).WithStatusCode(StatusCodes.Status404NotFound);
        }
        return dtos;
    }
    #endregion
}