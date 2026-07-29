using FluentResults;
using Mediator;
using Microsoft.AspNetCore.Identity;
using ToDoApp.Data;
using ToDoApp.Data.Features.Auth.Users;
using ToDoApp.Data.Features.Tasks;
using ToDoApp.Data.Features.Tasks_Categories;
using ToDoApp.WebAPI.Extensions;
using ToDoApp.WebAPI.Features.Tasks.Dtos;

namespace ToDoApp.WebAPI.Features.Tasks.Handlers;

public sealed class CreateTaskHandler(
    AppDbContext thisDbContext,
    ILogger<CreateTaskHandler> thisLogger,
    IJoinHandler<Task_Category_JoinEntity> thisJoinHandler,
    IHttpContextAccessor thisHttpContext,
    UserManager<ApplicationUser> thisUserManager
) : IRequestHandler<CreateTaskCommand, Result<Guid>>
{
    #region Interfaces
    public async ValueTask<Result<Guid>> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        if(await thisUserManager.GetUserAsync(thisHttpContext.HttpContext!.User) is not ApplicationUser user)
        {
            return Result.Fail("").WithStatusCode(StatusCodes.Status401Unauthorized).LogTo(thisLogger);
        }
        TaskEntity entity = request.Task.ToEntity();
        entity.Id = Guid.Empty;
        entity.UserId = user.Id;

        await thisDbContext.AddAsync(entity, cancellationToken);
        Result result = await thisJoinHandler.Handle([], entity.Categories, cancellationToken);
        if(result.IsFailed)
        {
            return result.ToResult<Guid>();
        }
        await thisDbContext.SaveChangesAsync(cancellationToken);
        return Result.Ok(entity.Id);
    }
    #endregion
}