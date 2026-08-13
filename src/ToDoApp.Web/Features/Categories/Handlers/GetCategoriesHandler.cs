using ErrorOr;
using Mediator;
using ToDoApp.Data;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Web.Features.Categories.Dtos;
using ToDoApp.Data.Features.Categories;
using ToDoApp.Web.Features.Categories;
using ToDoApp.Web.Shared.Behaviors;
using ToDoApp.Data.Features.Auth;

namespace ToDoApp.Web.Features.Categories.Handlers;

public sealed class GetCategoriesHandler(AppDbContext thisDbContext) : IQueryHandler<GetCategoriesQuery, ErrorOr<GetCategoriesResponse>>
{
    #region Interfaces
    public async ValueTask<ErrorOr<GetCategoriesResponse>> Handle(GetCategoriesQuery query, CancellationToken cancellationToken)
    {
        UserEntity user = query.User;
        IQueryable<CategoryEntity> entities = thisDbContext.Categories.AsNoTracking().Include(e => e.Tasks).Where(e => e.UserId == user.Id);
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
public sealed record class GetCategoriesResponse(IEnumerable<CategoryDto> Categories);
public sealed record class GetCategoriesQuery(Guid[] Ids) : IAuthorizedMessage, IQuery<ErrorOr<GetCategoriesResponse>>;