using Mediator;
using ToDoApp.Data;
using FluentResults;
using ToDoApp.WebAPI.Resources;
using ToDoApp.WebAPI.Extensions;
using ToDoApp.Data.Features.Tasks;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Data.Features.Categories;
using ToDoApp.WebAPI.Features.Tasks.Dtos;

namespace ToDoApp.WebAPI.Features.Tasks.Handlers;

public sealed class GetTasksByFilterHandler(
    AppDbContext thisDbContext,
    ILogger<GetTasksByFilterHandler> thisLogger
) : IRequestHandler<GetTasksByFilterQuery, Result<TaskDto[]>>
{
    #region Interfaces
    public async ValueTask<Result<TaskDto[]>> Handle(GetTasksByFilterQuery request, CancellationToken cancellationToken)
    {
        IQueryable<TaskEntity> query = thisDbContext.Tasks.AsNoTracking().Include(e => e.Categories);
        if(request.CategoryId is Guid id)
        {
            CategoryEntity? category = await thisDbContext.Categories.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
            if(category is null)
            {
                string msg = string.Format(ErrorMessages.RecordNotFound, nameof(CategoryEntity), nameof(CategoryEntity.Id), id);
                return Result.Fail(msg).LogTo(thisLogger).WithStatusCode(StatusCodes.Status404NotFound);
            }
            query = query.Where(e => e.Categories.Select(je => je.CategoryId).Contains(id));
        }
        if(request.Search is string search)
        {
            search = search.Trim();
            query = query.Where(e => e.Title.Contains(search) || e.Description.Contains(search));
        }
        if(request.Done is bool done)
        {
            query = query.Where(e => e.Done == done);
        }
        if(request.From is int from)
        {
            query = query.Skip(from);
        }
        if(request.Count is int count)
        {
            query = query.Take(count);
        }
        return await query.ProjectToDtos().ToArrayAsync(cancellationToken);
    }
    #endregion
}