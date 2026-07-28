using Mediator;
using ToDoApp.Data;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using ToDoApp.WebAPI.Features.Categories.Dtos;

namespace ToDoApp.WebAPI.Features.Categories.Handlers;

public sealed class GetAllCategoriesHandler(AppDbContext thisDbContext) : IRequestHandler<GetAllCategoriesQuery, Result<IEnumerable<CategoryDto>>>
{
    #region Interfaces
    public async ValueTask<Result<IEnumerable<CategoryDto>>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
    {
        List<CategoryDto> result = await thisDbContext.Categories.AsNoTracking().ProjectToDtos().ToListAsync(cancellationToken);
        return Result.Ok(result.AsEnumerable());
    }
    #endregion
}