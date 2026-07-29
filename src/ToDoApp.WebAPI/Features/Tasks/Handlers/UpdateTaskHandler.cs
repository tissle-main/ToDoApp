using FluentResults;
using Mediator;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Data;
using ToDoApp.Data.Features.Auth.Users;
using ToDoApp.Data.Features.Tasks;
using ToDoApp.Data.Features.Tasks_Categories;
using ToDoApp.WebAPI.Extensions;
using ToDoApp.WebAPI.Features.Tasks.Dtos;
using ToDoApp.WebAPI.Resources;

namespace ToDoApp.WebAPI.Features.Tasks.Handlers;

public sealed class UpdateTaskHandler(
    AppDbContext thisDbContext,
    ILogger<UpdateTaskHandler> thisLogger,
    IJoinHandler<Task_Category_JoinEntity> thisJoinHandler,
    IHttpContextAccessor thisHttpContext,
    UserManager<ApplicationUser> thisUserManager
) : IRequestHandler<UpdateTaskCommand, Result>
{
    #region Interfaces
    public async ValueTask<Result> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
    {
        if(await thisUserManager.GetUserAsync(thisHttpContext.HttpContext!.User) is not ApplicationUser user)
        {
            return Result.Fail("").WithStatusCode(StatusCodes.Status401Unauthorized).LogTo(thisLogger);
        }
        TaskEntity newEntity = request.Task.ToEntity();
        TaskEntity? oldEntity = await thisDbContext.Tasks.AsNoTracking()
            .Include(e => e.Categories)
            .Where(e => e.UserId == user.Id)
            .FirstOrDefaultAsync(e => e.Id == newEntity.Id, cancellationToken);
        if(oldEntity is null)
        {
            string msg = string.Format(ErrorMessages.RecordNotFound, nameof(TaskEntity), nameof(TaskEntity.Id), request.Task.Id);
            return Result.Fail(msg).LogTo(thisLogger).WithStatusCode(StatusCodes.Status404NotFound);
        }
        List<Task_Category_JoinEntity> oldList = oldEntity.Categories;
        List<Task_Category_JoinEntity> newList = newEntity.Categories;
        newEntity.UserId = oldEntity.UserId;
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