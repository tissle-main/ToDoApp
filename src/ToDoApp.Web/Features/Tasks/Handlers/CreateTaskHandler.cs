using ErrorOr;
using Mediator;
using ToDoApp.Data;
using ToDoApp.Data.Features.Tasks;
using ToDoApp.Web.Features.Tasks.Dtos;
using ToDoApp.Web.Features.Tasks_Categories.Handlers;
using ToDoApp.Web.Shared.Behaviors;

namespace ToDoApp.Web.Features.Tasks.Handlers;

public sealed class CreateTaskHandler(AppDbContext thisDbContext, IMediator thisMediator) : ICommandHandler<CreateTaskCommand, ErrorOr<CreateTaskResponse>>
{
    #region Interfaces
    public async ValueTask<ErrorOr<CreateTaskResponse>> Handle(CreateTaskCommand command, CancellationToken cancellationToken)
    {
        TaskEntity entity = command.Task.ToEntity();
        entity.UserId = command.User.Id;

        await thisDbContext.Tasks.AddAsync(entity, cancellationToken);
        ErrorOr<Unit> errorOnUnit = await thisMediator.Send(new Task_Category_UpdateCommand([], entity.Categories)
        {
            SaveDatabase = false
        }, cancellationToken);
        return errorOnUnit.Then(unit => new CreateTaskResponse(entity.Id)); 
    }
    #endregion
}
public sealed record class CreateTaskResponse(Guid CreatedId);
public sealed record class CreateTaskCommand(TaskDto Task) : IDbSaveMessage, IAuthorizedMessage, ICommand<ErrorOr<CreateTaskResponse>>;