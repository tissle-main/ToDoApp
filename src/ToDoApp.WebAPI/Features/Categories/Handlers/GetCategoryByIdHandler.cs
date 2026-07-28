using Mediator;
using ToDoApp.Data;
using FluentResults;
using ToDoApp.WebAPI.Resources;
using ToDoApp.WebAPI.Extensions;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Data.Features.Categories;
using ToDoApp.WebAPI.Features.Categories.Dtos;

namespace ToDoApp.WebAPI.Features.Categories.Handlers;

public sealed class GetTaskByIdHandler(
    AppDbContext thisDbContext,
    ILogger<GetCategoryByIdQuery> thisLogger
) : IRequestHandler<GetCategoryByIdQuery, Result<CategoryDto>>
{
    #region Interfaces
    public async ValueTask<Result<CategoryDto>> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        CategoryEntity? entity = await thisDbContext.Categories.AsNoTracking()
            .Include(e => e.Tasks).ThenInclude(je => je.Task)
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);
        if(entity is null)
        {
            string msg = string.Format(ErrorMessages.RecordNotFound, nameof(CategoryEntity), nameof(CategoryEntity.Id), request.Id);
            return Result.Fail(msg).LogTo(thisLogger).WithStatusCode(StatusCodes.Status404NotFound);
        }
        return Result.Ok(entity.ToDto());
    }
    #endregion
}