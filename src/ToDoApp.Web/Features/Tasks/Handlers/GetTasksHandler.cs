using ErrorOr;
using Mediator;
using ToDoApp.Data;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Web.Features.Tasks.Dtos;
using ToDoApp.Data.Features.Tasks;
using ToDoApp.Web.Features.Tasks;
using ToDoApp.Web.Shared.Behaviors;
using ToDoApp.Data.Features.Auth;

namespace ToDoApp.Web.Features.Tasks.Handlers;

public sealed class GetTasksHandler(AppDbContext thisDbContext) : IQueryHandler<GetTasksQuery, ErrorOr<GetTasksResponse>>
{
    #region Interfaces
    public async ValueTask<ErrorOr<GetTasksResponse>> Handle(GetTasksQuery query, CancellationToken cancellationToken)
    {
        UserEntity user = query.User;
        IQueryable<TaskEntity> entities = thisDbContext.Tasks.AsNoTracking().Include(e => e.Categories).Where(e => e.UserId == user.Id);
        if(query.Ids.Length == 0)
        {
            return new GetTasksResponse(await entities.ProjectToDto().ToArrayAsync(cancellationToken));
        }
        TaskDto[] dtos = await entities.Where(e => query.Ids.Contains(e.Id)).ProjectToDto().ToArrayAsync(cancellationToken);
        if(dtos.Length != query.Ids.Length)
        {
            return TaskErrors.NotFound();
        }
        return new GetTasksResponse(dtos);
    }
    #endregion
}
public sealed record class GetTasksResponse(IEnumerable<TaskDto> Tasks);
public sealed record class GetTasksQuery(Guid[] Ids) : IAuthorizedMessage, IQuery<ErrorOr<GetTasksResponse>>;