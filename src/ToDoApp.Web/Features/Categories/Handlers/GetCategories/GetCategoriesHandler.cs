using ErrorOr;
using Mediator;
using ToDoApp.Data;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Data.Features.Auth.Users;
using ToDoApp.Data.Features.Categories;
using ToDoApp.Web.Features.Categories.Dtos;
using ToDoApp.Web.Shared.Behaviors.Authorized;

namespace ToDoApp.Web.Features.Categories.Handlers.GetCategories;

public sealed class GetCategoriesHandler(AppDbContext thisDbContext) : IQueryHandler<GetCategoriesQuery, ErrorOr<GetCategoriesResponse>>
{
    #region Interfaces
    public async ValueTask<ErrorOr<GetCategoriesResponse>> Handle(GetCategoriesQuery query, CancellationToken cancellationToken)
    {
        UserEntity user = query.User;
        IQueryable<CategoryEntity> entities = thisDbContext.Categories.Include(e => e.Tasks).Where(e => e.UserId == user.Id);
        if(query.Ids.Length == 0)
        {
            return new GetCategoriesResponse(await entities.ProjectToDto().ToArrayAsync(cancellationToken));
        }
        else
        {
            CategoryDto[] dtos = await entities.Where(e => query.Ids.Contains(e.Id)).ProjectToDto().ToArrayAsync(cancellationToken);
            if(dtos.Length != query.Ids.Length)
            {
                return CategoryErrors.NotFound();
            }
            return new GetCategoriesResponse(dtos);
        }
    }
    #endregion
}