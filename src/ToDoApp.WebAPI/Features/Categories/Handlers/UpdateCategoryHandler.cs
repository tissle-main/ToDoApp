using FluentResults;
using Mediator;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Data;
using ToDoApp.Data.Features.Auth.Users;
using ToDoApp.Data.Features.Categories;
using ToDoApp.Data.Features.Tasks_Categories;
using ToDoApp.WebAPI.Extensions;
using ToDoApp.WebAPI.Features.Categories.Dtos;
using ToDoApp.WebAPI.Resources;

namespace ToDoApp.WebAPI.Features.Categories.Handlers;

public sealed class UpdateCategoryHandler(
    AppDbContext thisDbContext,
    ILogger<UpdateCategoryHandler> thisLogger,
    IJoinHandler<Task_Category_JoinEntity> thisJoinHandler,
    IHttpContextAccessor thisHttpContext,
    UserManager<ApplicationUser> thisUserManager
) : IRequestHandler<UpdateCategoryCommand, Result>
{
    #region Interfaces
    public async ValueTask<Result> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        if(await thisUserManager.GetUserAsync(thisHttpContext.HttpContext!.User) is not ApplicationUser user)
        {
            return Result.Fail("").WithStatusCode(StatusCodes.Status401Unauthorized).LogTo(thisLogger);
        }
        CategoryEntity newEntity = request.Category.ToEntity();
        CategoryEntity? oldEntity = await thisDbContext.Categories.AsNoTracking().Where(c => c.UserId == user.Id)
            .Include(e => e.Tasks)
            .FirstOrDefaultAsync(e => e.Id == newEntity.Id, cancellationToken);
        if(oldEntity is null)
        {
            string msg = string.Format(ErrorMessages.RecordNotFound, nameof(CategoryEntity), nameof(CategoryEntity.Id), request.Category.Id);
            return Result.Fail(msg).LogTo(thisLogger).WithStatusCode(StatusCodes.Status404NotFound);
        }
        List<Task_Category_JoinEntity> oldList = oldEntity.Tasks;
        List<Task_Category_JoinEntity> newList = newEntity.Tasks;
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