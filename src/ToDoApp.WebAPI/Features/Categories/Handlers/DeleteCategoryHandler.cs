using Mediator;
using ToDoApp.Data;
using FluentResults;
using ToDoApp.WebAPI.Resources;
using ToDoApp.WebAPI.Extensions;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Data.Features.Categories;

namespace ToDoApp.WebAPI.Features.Categories.Handlers;

public sealed class DeleteCategoryHandler(
    AppDbContext thisDbContext,
    ILogger<DeleteCategoryHandler> thisLogger
) : IRequestHandler<DeleteCategoryCommand, Result>
{
    #region Interfaces
    public async ValueTask<Result> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        CategoryEntity? entity = await thisDbContext.Categories.AsNoTracking().FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);
        if(entity is null)
        {
            string msg = string.Format(ErrorMessages.RecordNotFound, nameof(CategoryEntity), nameof(CategoryEntity.Id), request.Id);
            return Result.Fail(msg).LogTo(thisLogger).WithStatusCode(StatusCodes.Status404NotFound);
        }
        thisDbContext.Remove(entity);
        await thisDbContext.SaveChangesAsync(cancellationToken);
        return Result.Ok();
    }
    #endregion
}