using ErrorOr;
using Mediator;
using ToDoApp.Data;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Data.Features.Categories;
using ToDoApp.Web.Features.Categories.Dtos;
using ToDoApp.Web.Features.Tasks_Categories.Handlers;
using ToDoApp.Web.Shared.Behaviors;
using ToDoApp.Data.Features.Auth;

namespace ToDoApp.Web.Features.Categories.Handlers;

public sealed class UpdateCategoryHandler(AppDbContext thisDbContext, IMediator thisMediator) : ICommandHandler<UpdateCategoryCommand, ErrorOr<Unit>>
{
    #region Interfaces
    public async ValueTask<ErrorOr<Unit>> Handle(UpdateCategoryCommand command, CancellationToken cancellationToken)
    {
        UserEntity user = command.User;
        CategoryEntity newEntity = command.Category.ToEntity();
        CategoryEntity? oldEntity = await thisDbContext.Categories.AsNoTracking()
            .Include(e => e.Tasks)
            .Where(e => e.UserId == user.Id)
            .FirstOrDefaultAsync(e => e.Id == newEntity.Id, cancellationToken);
        if(oldEntity is null)
        {
            return CategoryErrors.NotFound();
        }
        
        ErrorOr<Unit> errorOnUnit = await thisMediator.Send(
            new Task_Category_UpdateCommand(oldEntity.Tasks, newEntity.Tasks)
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
        thisDbContext.Categories.Update(oldEntity);
        return Unit.Value;
    }
    #endregion
}
public sealed record class UpdateCategoryCommand(CategoryDto Category) : IDbSaveMessage, IAuthorizedMessage, ICommand<ErrorOr<Unit>>;