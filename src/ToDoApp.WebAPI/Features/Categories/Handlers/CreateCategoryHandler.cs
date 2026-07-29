using Mediator;
using ToDoApp.Data;
using FluentResults;
using ToDoApp.Data.Features.Categories;
using ToDoApp.WebAPI.Features.Tasks.Dtos;
using ToDoApp.Data.Features.Tasks_Categories;
using ToDoApp.WebAPI.Features.Categories.Dtos;

namespace ToDoApp.WebAPI.Features.Categories.Handlers;

public sealed class CreateCategoryHandler(
    AppDbContext thisDbContext,
    IJoinHandler<Task_Category_JoinEntity> thisJoinHandler
) : IRequestHandler<CreateCategoryCommand, Result<Guid>>
{
    #region Interfaces
    public async ValueTask<Result<Guid>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        CategoryEntity entity = request.Category.ToEntity();
        entity.Id = Guid.Empty;

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