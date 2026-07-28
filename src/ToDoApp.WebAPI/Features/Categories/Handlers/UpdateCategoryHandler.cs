using Mediator;
using ToDoApp.Data;
using FluentResults;
using ToDoApp.WebAPI.Resources;
using ToDoApp.WebAPI.Extensions;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Data.Features.Categories;
using ToDoApp.Data.Features.Tasks_Categories;
using ToDoApp.WebAPI.Features.Categories.Dtos;

namespace ToDoApp.WebAPI.Features.Categories.Handlers;

public sealed class UpdateCategoryHandler(
    AppDbContext thisDbContext,
    ILogger<UpdateCategoryHandler> thisLogger,
    IJoinHandler<Task_Category_JoinEntity> thisJoinHandler
) : IRequestHandler<UpdateCategoryCommand, Result>
{
    #region Interfaces
    public async ValueTask<Result> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        CategoryEntity newEntity = request.Category.ToEntity();
        CategoryEntity? oldEntity = await thisDbContext.Categories.AsNoTracking()
            .Include(e => e.Tasks)
            .FirstOrDefaultAsync(e => e.Id == newEntity.Id, cancellationToken);
        if(oldEntity is null)
        {
            string msg = string.Format(ErrorMessages.RecordNotFound, nameof(CategoryEntity), nameof(CategoryEntity.Id), request.Category.Id);
            return Result.Fail(msg).LogTo(thisLogger).WithStatusCode(StatusCodes.Status404NotFound);
        }
        List<Task_Category_JoinEntity> oldList = oldEntity.Tasks;
        List<Task_Category_JoinEntity> newList = newEntity.Tasks;
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