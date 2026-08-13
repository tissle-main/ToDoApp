using ErrorOr;
using Mediator;
using ToDoApp.Data;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Data.Features.Categories;
using ToDoApp.Web.Shared.Behaviors;
using ToDoApp.Data.Features.Auth;

namespace ToDoApp.Web.Features.Categories.Handlers;

public sealed class DeleteCategoriesHandler(AppDbContext thisDbContext) : ICommandHandler<DeleteCategoriesCommand, ErrorOr<Unit>>
{
    #region Interfaces
    public async ValueTask<ErrorOr<Unit>> Handle(DeleteCategoriesCommand command, CancellationToken cancellationToken)
    {
        UserEntity user = command.User;
        IQueryable<CategoryEntity> entities = thisDbContext.Categories.AsNoTracking().Where(e => e.UserId == user.Id);
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
        return Unit.Value;
    }
    #endregion
}
public sealed record class DeleteCategoriesCommand(Guid[] Ids) : IDbSaveMessage, IAuthorizedMessage, ICommand<ErrorOr<Unit>>;