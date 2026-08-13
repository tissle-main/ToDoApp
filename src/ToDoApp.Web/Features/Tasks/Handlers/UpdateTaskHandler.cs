using ErrorOr;
using Mediator;
using ToDoApp.Data;
using ToDoApp.Web.Features.Tasks;
using ToDoApp.Data.Features.Tasks;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Web.Features.Tasks.Dtos;
using ToDoApp.Web.Features.Tasks_Categories.Handlers;
using ToDoApp.Web.Shared.Behaviors;
using ToDoApp.Data.Features.Auth;

namespace ToDoApp.Web.Features.Tasks.Handlers;

public sealed class UpdateTaskHandler(AppDbContext thisDbContext, IMediator thisMediator) : ICommandHandler<UpdateTaskCommand, ErrorOr<Unit>>
{
    #region Interfaces
    public async ValueTask<ErrorOr<Unit>> Handle(UpdateTaskCommand command, CancellationToken cancellationToken)
    {
        UserEntity user = command.User;
        TaskEntity newEntity = command.Task.ToEntity();
        TaskEntity? oldEntity = await thisDbContext.Tasks.AsNoTracking()
            .Include(e => e.Categories)
            .Where(e => e.UserId == user.Id)
            .FirstOrDefaultAsync(e => e.Id == newEntity.Id, cancellationToken);
        if(oldEntity is null)
        {
            return TaskErrors.NotFound();
        }
        
        ErrorOr<Unit> errorOnUnit = await thisMediator.Send(
            new Task_Category_UpdateCommand(oldEntity.Categories, newEntity.Categories)
            {
                SaveDatabase = false
            },
            cancellationToken
        );
        if(errorOnUnit.IsError)
        {
            return errorOnUnit;
        }

        newEntity.MapToEntity(oldEntity);
        thisDbContext.Tasks.Update(oldEntity);
        return Unit.Value;
    }
    #endregion
}
public sealed record class UpdateTaskCommand(TaskDto Task) : IDbSaveMessage, IAuthorizedMessage, ICommand<ErrorOr<Unit>>;