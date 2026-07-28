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

public sealed class GetTasksHandler(
    AppDbContext thisDbContext,
    ILogger<GetTasksHandler> thisLogger
) : IRequestHandler<GetTasksQuery, Result<IEnumerable<TaskDto>>>
{
    #region Interfaces
    public async ValueTask<Result<IEnumerable<TaskDto>>> Handle(GetTasksQuery request, CancellationToken cancellationToken)
    {
        IQueryable<TaskEntity> query;
        if(request.CategoryId is Guid id)
        {
            CategoryEntity? category = await thisDbContext.Categories.AsNoTracking()
                .Include(e => e.Tasks).ThenInclude(je => je.Task)
                .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
            if(category is null)
            {
                string msg = string.Format(ErrorMessages.RecordNotFound, nameof(CategoryEntity), nameof(CategoryEntity.Id), id);
                return Result.Fail(msg).LogTo(thisLogger).WithStatusCode(StatusCodes.Status404NotFound);
            }
            query = category.Tasks.Select(je => je.Task!).AsQueryable();
        }
        else
        {
            query = thisDbContext.Tasks.AsNoTracking();
        }
        query = query.Include(e => e.Categories).ThenInclude(je => je.Category);
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
        List<TaskDto> result = await query.ProjectToDtos().ToListAsync(cancellationToken);
        return Result.Ok(result.AsEnumerable());
    }
    #endregion
}