using ErrorOr;
using Mediator;
using ToDoApp.Data;
using ToDoApp.Data.Features.Tasks;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Web.Features.Tasks.Dtos;
using ToDoApp.Data.Features.Auth.Users;
using ToDoApp.Web.Shared.Behaviors.Authorized;

namespace ToDoApp.Web.Features.Tasks.Handlers.GetTasks;

public sealed class GetTasksHandler(AppDbContext thisDbContext) : IQueryHandler<GetTasksQuery, ErrorOr<GetTasksResponse>>
{
    #region Interfaces
    public async ValueTask<ErrorOr<GetTasksResponse>> Handle(GetTasksQuery query, CancellationToken cancellationToken)
    {
        UserEntity user = query.User;
        IQueryable<TaskEntity> entities = thisDbContext.Tasks.Include(e => e.Categories).Where(e => e.UserId == user.Id);
        if(query.Ids.Length == 0)
        {
            return new GetTasksResponse(await entities.OrderByDescending(e => e.CreatedAt).ProjectToDto().ToArrayAsync(cancellationToken));
        }
        TaskDto[] dtos = await entities.Where(e => query.Ids.Contains(e.Id)).OrderByDescending(e => e.CreatedAt).ProjectToDto().ToArrayAsync(cancellationToken);
        if(dtos.Length != query.Ids.Length)
        {
            return TaskErrors.NotFound();
        }
        return new GetTasksResponse(dtos);
    }
    #endregion
}