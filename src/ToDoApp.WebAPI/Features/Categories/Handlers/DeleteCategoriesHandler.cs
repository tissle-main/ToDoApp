using Mediator;
using ToDoApp.Data;
using FluentResults;
using ToDoApp.WebAPI.Resources;
using ToDoApp.WebAPI.Extensions;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Data.Features.Categories;

namespace ToDoApp.WebAPI.Features.Categories.Handlers;

public sealed class DeleteCategoriesHandler(
    AppDbContext thisDbContext,
    ILogger<DeleteCategoriesHandler> thisLogger
) : IRequestHandler<DeleteCategoriesCommand, Result>
{
    #region Interfaces
    public async ValueTask<Result> Handle(DeleteCategoriesCommand request, CancellationToken cancellationToken)
    {
        IQueryable<CategoryEntity> query = thisDbContext.Categories.AsNoTracking();
        if(request.Ids.Length == 0)
        {
            thisDbContext.RemoveRange(await query.ToArrayAsync(cancellationToken));
            await thisDbContext.SaveChangesAsync(cancellationToken);
            return Result.Ok();
        }
        CategoryEntity[] entities = await query.Where(e => request.Ids.Contains(e.Id)).ToArrayAsync(cancellationToken);
        if(entities.Length != request.Ids.Length)
        {
            Guid[] missingIds = request.Ids.Except(entities.Select(dto => dto.Id)).ToArray();
            string idsString = string.Join(", ", missingIds);
            string msg = string.Format(ErrorMessages.RecordsNotFound, nameof(CategoryEntity), nameof(CategoryEntity.Id), idsString);
            return Result.Fail(msg).LogTo(thisLogger).WithStatusCode(StatusCodes.Status404NotFound);
        }
        thisDbContext.Categories.RemoveRange(entities);
        await thisDbContext.SaveChangesAsync(cancellationToken);
        return Result.Ok();
    }
    #endregion
}