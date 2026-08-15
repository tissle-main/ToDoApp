using ErrorOr;
using Mediator;
using ToDoApp.Data;
using ToDoApp.Data.Features.Tasks;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Web.Features.Tasks.Dtos;
using ToDoApp.Data.Features.Auth.Users;
using ToDoApp.Web.Shared.Behaviors.Authorized;

namespace ToDoApp.Web.Features.Tasks.Handlers.GetTasksByFilter;

public sealed class GetTasksByFilterHandler(AppDbContext thisDbContext) : IQueryHandler<GetTasksByFilterQuery, ErrorOr<GetTasksByFilterResponse>>
{
    #region Interfaces
    public async ValueTask<ErrorOr<GetTasksByFilterResponse>> Handle(GetTasksByFilterQuery query, CancellationToken cancellationToken)
    {
        UserEntity user = query.User;
        IQueryable<TaskEntity> tasks = thisDbContext.Tasks.Where(e => e.UserId == user.Id);
        if(query.Category is not null)
        {
            tasks = tasks.Include(e => e.Categories).ThenInclude(je => je.Right);
        }
        else
        {
            tasks = tasks.Include(e => e.Categories);
        }

        if(query.Search is string search)
        {
            tasks = tasks.Where(e =>
                e.Title.Contains(search) || (e.Description != null && e.Description.Contains(search))
            );
        }
        if(query.Category is string category)
        {
            tasks = tasks.Where(e => e.Categories.Any(je => je.Right!.Name == category));
        }
        if(query.Done is bool done)
        {
            tasks = tasks.Where(e => e.Done == done);
        }
        if(query.Skip is int skip)
        {
            tasks = tasks.Skip(skip);
        }
        if(query.Take is int take)
        {
            tasks = tasks.Take(take);
        }
        return new GetTasksByFilterResponse(await tasks.ProjectToDto().ToArrayAsync(cancellationToken));
    }
    #endregion
}