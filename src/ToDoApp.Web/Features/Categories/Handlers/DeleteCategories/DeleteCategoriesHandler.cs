using ErrorOr;
using Mediator;
using ToDoApp.Data;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Data.Features.Categories;
using ToDoApp.Data.Features.Auth.Users;
using ToDoApp.Web.Shared.Behaviors.Authorized;

namespace ToDoApp.Web.Features.Categories.Handlers.DeleteCategories;

public sealed class DeleteCategoriesHandler(AppDbContext thisDbContext) : ICommandHandler<DeleteCategoriesCommand, ErrorOr<Unit>>
{
    #region Interfaces
    public async ValueTask<ErrorOr<Unit>> Handle(DeleteCategoriesCommand command, CancellationToken cancellationToken)
    {
        UserEntity user = command.User;
        IQueryable<CategoryEntity> entities = thisDbContext.Categories.Where(e => e.UserId == user.Id);
        CategoryEntity[] categories;
        if(command.Ids.Length > 0)
        {
            categories = await entities.Where(e => command.Ids.Contains(e.Id)).ToArrayAsync(cancellationToken);
            if(categories.Length != command.Ids.Length)
            {
                return CategoryErrors.NotFound();
            }
        }
        else
        {
            categories = await entities.ToArrayAsync(cancellationToken);
        }
        thisDbContext.Categories.RemoveRange(categories);
        await thisDbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
    #endregion
}