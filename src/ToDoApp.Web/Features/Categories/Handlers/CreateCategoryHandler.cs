using ErrorOr;
using Mediator;
using ToDoApp.Data;
using ToDoApp.Data.Features.Categories;
using ToDoApp.Web.Features.Categories.Dtos;
using ToDoApp.Web.Features.Tasks_Categories.Handlers;
using ToDoApp.Web.Shared.Behaviors;

namespace ToDoApp.Web.Features.Categories.Handlers;

public sealed class CreateCategoryHandler(AppDbContext thisDbContext, IMediator thisMediator) : ICommandHandler<CreateCategoryCommand, ErrorOr<CreateCategoryResponse>>
{
    #region Interfaces
    public async ValueTask<ErrorOr<CreateCategoryResponse>> Handle(CreateCategoryCommand command, CancellationToken cancellationToken)
    {
        CategoryEntity entity = command.Category.ToEntity();
        entity.UserId = command.User.Id;

        await thisDbContext.Categories.AddAsync(entity, cancellationToken);
        ErrorOr<Unit> errorOnUnit = await thisMediator.Send(new Task_Category_UpdateCommand([], entity.Tasks)
        {
            SaveDatabase = false
        }, cancellationToken);
        return errorOnUnit.Then(unit => new CreateCategoryResponse(entity.Id)); 
    }
    #endregion
}
public sealed record class CreateCategoryResponse(Guid CreatedId);
public sealed record class CreateCategoryCommand(CategoryDto Category) : IDbSaveMessage, IAuthorizedMessage, ICommand<ErrorOr<CreateCategoryResponse>>;