using ErrorOr;
using Mediator;
using ToDoApp.Data;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Data.Features.Auth.Users;
using ToDoApp.Data.Features.Categories;
using ToDoApp.Web.Features.Categories.Dtos;
using ToDoApp.Web.Features.Tasks_Categories;
using ToDoApp.Web.Shared.Behaviors.Authorized;
using ToDoApp.Web.Shared.Behaviors.DbTransaction;

namespace ToDoApp.Web.Features.Categories.Handlers.UpdateCategory;

public sealed class UpdateCategoryHandler(AppDbContext thisDbContext, IMediator thisMediator) : ICommandHandler<UpdateCategoryCommand, ErrorOr<Unit>>
{
    #region Interfaces
    public async ValueTask<ErrorOr<Unit>> Handle(UpdateCategoryCommand command, CancellationToken cancellationToken)
    {
        UserEntity user = command.User;
        CategoryEntity newEntity = command.Category.ToEntity();
        CategoryEntity? oldEntity = await thisDbContext.Categories.Include(e => e.Tasks).Where(e => e.UserId == user.Id).FirstOrDefaultAsync(
            e => e.Id == newEntity.Id,
            cancellationToken
        );
        if(oldEntity is null)
        {
            return CategoryErrors.NotFound();
        }
        
        ErrorOr<Unit> errorOnUnit = await thisMediator.Send(
            new Task_Category_UpdateCommand(oldEntity.Tasks, newEntity.Tasks)
            {
                BeginDbTransaction = false
            },
            cancellationToken
        );
        if(errorOnUnit.IsError)
        {
            return errorOnUnit;
        }

        command.Category.MapToEntity(oldEntity);
        thisDbContext.Categories.Update(oldEntity);
        await thisDbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
    #endregion
}