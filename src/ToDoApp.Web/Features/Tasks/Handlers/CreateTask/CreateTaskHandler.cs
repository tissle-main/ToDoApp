using ErrorOr;
using Mediator;
using ToDoApp.Data;
using ToDoApp.Data.Features.Tasks;
using ToDoApp.Web.Features.Tasks.Dtos;
using ToDoApp.Web.Features.Tasks_Categories;
using ToDoApp.Data.Features.Tasks_Categories;
using ToDoApp.Web.Shared.Behaviors.Authorized;
using ToDoApp.Web.Shared.Behaviors.DbTransaction;

namespace ToDoApp.Web.Features.Tasks.Handlers.CreateTask;

public sealed class CreateTaskHandler(AppDbContext thisDbContext, IMediator thisMediator) : ICommandHandler<CreateTaskCommand, ErrorOr<CreateTaskResponse>>
{
    #region Interfaces
    public async ValueTask<ErrorOr<CreateTaskResponse>> Handle(CreateTaskCommand command, CancellationToken cancellationToken)
    {
        TaskEntity entity = command.Task.ToEntity();
        List<Task_Category_JoinEntity> newEntities = entity.Categories;
        entity.UserId = command.User.Id;
        entity.Categories = [];

        await thisDbContext.Tasks.AddAsync(entity, cancellationToken);
        await thisDbContext.SaveChangesAsync(cancellationToken);
        
        foreach(Task_Category_JoinEntity je in newEntities)
        {
            je.RightId = entity.Id;
        }
        ErrorOr<Unit> errorOnUnit = await thisMediator.Send(new Task_Category_UpdateCommand([], newEntities)
        {
            BeginDbTransaction = false
        }, cancellationToken);
        return errorOnUnit.Then(unit => new CreateTaskResponse(entity.Id)); 
    }
    #endregion
}