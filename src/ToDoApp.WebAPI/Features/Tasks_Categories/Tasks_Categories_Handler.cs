using ToDoApp.Data;
using FluentResults;
using ToDoApp.WebAPI.Resources;
using ToDoApp.WebAPI.Extensions;
using ToDoApp.Data.Features.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Data.Features.Categories;
using ToDoApp.Data.Features.Auth.Users;
using ToDoApp.Data.Features.Tasks_Categories;

namespace ToDoApp.WebAPI.Features.Tasks_Categories;

public sealed class Tasks_Categories_JoinHandler(
    AppDbContext thisDbContext,
    ILogger<Tasks_Categories_JoinHandler> thisLogger,
    IHttpContextAccessor thisHttpContext,
    UserManager<ApplicationUser> thisUserManager
) : IJoinHandler<Task_Category_JoinEntity>
{
    #region Interfaces
    public async ValueTask<Result> Handle(
        List<Task_Category_JoinEntity> oldList,
        List<Task_Category_JoinEntity> newList,
        CancellationToken cancellationToken
    )
    {
        if(await thisUserManager.GetUserAsync(thisHttpContext.HttpContext!.User) is not ApplicationUser user)
        {
            return Result.Fail("").WithStatusCode(StatusCodes.Status401Unauthorized).LogTo(thisLogger);
        }
        if(oldList.Count == 0 && newList.Count == 0)
        {
            return Result.Ok();
        }
        if(newList.Count > 0)
        {
            Guid[] ids = newList.Select(e => e.TaskId).Distinct().ToArray();
            HashSet<Guid> existingIds = await thisDbContext.Tasks.AsNoTracking()
                .Where(t => t.UserId == user.Id)
                .Where(t => ids.Contains(t.Id))
                .Select(t => t.Id)
                .ToHashSetAsync(cancellationToken);
            Guid[] missingIds = ids.Except(existingIds).ToArray();
            if(missingIds.Length > 0)
            {
                string joinIds = string.Join(", ", missingIds);
                string msg = string.Format(ErrorMessages.RecordsNotFound, nameof(TaskEntity), nameof(TaskEntity.Id), joinIds);
                return Result.Fail(msg).LogTo(thisLogger).WithStatusCode(StatusCodes.Status404NotFound);
            }

            ids = newList.Select(e => e.CategoryId).Distinct().ToArray();
            existingIds = await thisDbContext.Categories.AsNoTracking()
                .Where(t => t.UserId == user.Id)
                .Where(t => ids.Contains(t.Id))
                .Select(t => t.Id)
                .ToHashSetAsync(cancellationToken);
            missingIds = ids.Except(existingIds).ToArray();
            if(missingIds.Length > 0)
            {
                string joinIds = string.Join(", ", missingIds);
                string msg = string.Format(ErrorMessages.RecordsNotFound, nameof(CategoryEntity), nameof(CategoryEntity.Id), joinIds);
                return Result.Fail(msg).LogTo(thisLogger).WithStatusCode(StatusCodes.Status404NotFound);
            }
        }
        thisDbContext.Tasks_Categories.RemoveRange(oldList.Except(newList));
        await thisDbContext.Tasks_Categories.AddRangeAsync(newList.Except(oldList), cancellationToken);
        await thisDbContext.SaveChangesAsync(cancellationToken);
        return Result.Ok();
    }
    #endregion
}