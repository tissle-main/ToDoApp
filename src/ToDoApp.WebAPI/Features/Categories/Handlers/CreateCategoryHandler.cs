using Mediator;
using ToDoApp.Data;
using FluentResults;
using ToDoApp.WebAPI.Extensions;
using Microsoft.AspNetCore.Identity;
using ToDoApp.Data.Features.Auth.Users;
using ToDoApp.Data.Features.Categories;
using ToDoApp.WebAPI.Features.Tasks.Dtos;
using ToDoApp.Data.Features.Tasks_Categories;
using ToDoApp.WebAPI.Features.Categories.Dtos;

namespace ToDoApp.WebAPI.Features.Categories.Handlers;

public sealed class CreateCategoryHandler(
    AppDbContext thisDbContext,
    ILogger<CreateCategoryHandler> thisLogger,
    IJoinHandler<Task_Category_JoinEntity> thisJoinHandler,
    IHttpContextAccessor thisHttpContext,
    UserManager<ApplicationUser> thisUserManager
) : IRequestHandler<CreateCategoryCommand, Result<Guid>>
{
    #region Interfaces
    public async ValueTask<Result<Guid>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        if(await thisUserManager.GetUserAsync(thisHttpContext.HttpContext!.User) is not ApplicationUser user)
        {
            return Result.Fail("").WithStatusCode(StatusCodes.Status401Unauthorized).LogTo(thisLogger);
        }
        CategoryEntity entity = request.Category.ToEntity();
        entity.Id = Guid.Empty;
        entity.UserId = user.Id;

        await thisDbContext.AddAsync(entity, cancellationToken);
        Result result = await thisJoinHandler.Handle([], entity.Tasks, cancellationToken);
        if(result.IsFailed)
        {
            return result;
        }
        await thisDbContext.SaveChangesAsync(cancellationToken);
        return Result.Ok(entity.Id);
    }
    #endregion
}