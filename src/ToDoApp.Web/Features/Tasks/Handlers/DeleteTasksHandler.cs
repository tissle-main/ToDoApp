using ErrorOr;
using Mediator;
using ToDoApp.Data;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Data.Features.Tasks;
using ToDoApp.Web.Features.Tasks;
using ToDoApp.Web.Shared.Behaviors;
using ToDoApp.Data.Features.Auth;

namespace ToDoApp.Web.Features.Tasks.Handlers;

public sealed class DeleteTasksHandler(AppDbContext thisDbContext) : ICommandHandler<DeleteTasksCommand, ErrorOr<Unit>>
{
    #region Interfaces
    public async ValueTask<ErrorOr<Unit>> Handle(DeleteTasksCommand command, CancellationToken cancellationToken)
    {
        UserEntity user = command.User;
        IQueryable<TaskEntity> entities = thisDbContext.Tasks.AsNoTracking().Where(e => e.UserId == user.Id);
        TaskEntity[] tasks;
        if(command.Ids.Length > 0)
        {
            tasks = await entities.Where(e => command.Ids.Contains(e.Id)).ToArrayAsync(cancellationToken);
            if(tasks.Length != command.Ids.Length)
            {
                return TaskErrors.NotFound();
            }
        }
        else
        {
            tasks = await entities.ToArrayAsync(cancellationToken);
        }
        thisDbContext.Tasks.RemoveRange(tasks);
        return Unit.Value;
    }
    #endregion
}
public sealed record class DeleteTasksCommand(Guid[] Ids) : IDbSaveMessage, IAuthorizedMessage, ICommand<ErrorOr<Unit>>;