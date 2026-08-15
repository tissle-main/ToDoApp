using ErrorOr;
using Mediator;
using ToDoApp.Data;
using ToDoApp.Data.Features.Tasks;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Web.Features.Tasks.Dtos;
using ToDoApp.Data.Features.Auth.Users;
using ToDoApp.Web.Features.Tasks_Categories;
using ToDoApp.Web.Shared.Behaviors.Authorized;
using ToDoApp.Web.Shared.Behaviors.DbTransaction;

namespace ToDoApp.Web.Features.Tasks.Handlers.UpdateTask;

public sealed class UpdateTaskHandler(AppDbContext thisDbContext, IMediator thisMediator) : ICommandHandler<UpdateTaskCommand, ErrorOr<Unit>>
{
    #region Interfaces
    public async ValueTask<ErrorOr<Unit>> Handle(UpdateTaskCommand command, CancellationToken cancellationToken)
    {
        UserEntity user = command.User;
        TaskEntity newEntity = command.Task.ToEntity();
        TaskEntity? oldEntity = await thisDbContext.Tasks.Include(e => e.Categories).Where(e => e.UserId == user.Id).FirstOrDefaultAsync(
            e => e.Id == newEntity.Id,
            cancellationToken
        );
        if(oldEntity is null)
        {
            return TaskErrors.NotFound();
        }
        
        ErrorOr<Unit> errorOnUnit = await thisMediator.Send(
            new Task_Category_UpdateCommand(oldEntity.Categories, newEntity.Categories)
            {
                BeginDbTransaction = false
            },
            cancellationToken
        );
        if(errorOnUnit.IsError)
        {
            return errorOnUnit;
        }

        command.Task.MapToEntity(oldEntity);
        thisDbContext.Tasks.Update(oldEntity);
        await thisDbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
    #endregion
}