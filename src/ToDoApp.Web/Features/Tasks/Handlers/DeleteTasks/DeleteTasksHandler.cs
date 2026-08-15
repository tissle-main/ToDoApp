using ErrorOr;
using Mediator;
using ToDoApp.Data;
using ToDoApp.Data.Features.Tasks;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Data.Features.Auth.Users;
using ToDoApp.Web.Shared.Behaviors.Authorized;

namespace ToDoApp.Web.Features.Tasks.Handlers.DeleteTasks;

public sealed class DeleteTasksHandler(AppDbContext thisDbContext) : ICommandHandler<DeleteTasksCommand, ErrorOr<Unit>>
{
    #region Interfaces
    public async ValueTask<ErrorOr<Unit>> Handle(DeleteTasksCommand command, CancellationToken cancellationToken)
    {
        UserEntity user = command.User;
        IQueryable<TaskEntity> entities = thisDbContext.Tasks.Where(e => e.UserId == user.Id);
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
        await thisDbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
    #endregion
}