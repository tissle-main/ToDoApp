using Mediator;
using ToDoApp.Data;
using FluentResults;
using ToDoApp.WebAPI.Resources;
using ToDoApp.WebAPI.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Data.Features.Auth.Users;
using ToDoApp.Data.Features.Categories;
using ToDoApp.WebAPI.Features.Categories.Dtos;

namespace ToDoApp.WebAPI.Features.Categories.Handlers;

public sealed class GetCategoriesHandler(
    AppDbContext thisDbContext,
    ILogger<GetCategoriesHandler> thisLogger,
    IHttpContextAccessor thisHttpContext,
    UserManager<ApplicationUser> thisUserManager
) : IRequestHandler<GetCategoriesQuery, Result<CategoryDto[]>>
{
    #region Interfaces
    public async ValueTask<Result<CategoryDto[]>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        if(await thisUserManager.GetUserAsync(thisHttpContext.HttpContext!.User) is not ApplicationUser user)
        {
            return Result.Fail("").WithStatusCode(StatusCodes.Status401Unauthorized).LogTo(thisLogger);
        }
        IQueryable<CategoryEntity> query = thisDbContext.Categories.AsNoTracking().Include(e => e.Tasks).Where(e => e.UserId == user.Id);
        if(request.Ids.Length == 0)
        {
            return await query.ProjectToDtos().ToArrayAsync(cancellationToken);
        }
        CategoryDto[] dtos = await query.Where(e => request.Ids.Contains(e.Id)).ProjectToDtos().ToArrayAsync(cancellationToken);
        if(dtos.Length != request.Ids.Length)
        {
            Guid[] missingIds = request.Ids.Except(dtos.Select(dto => dto.Id)).ToArray();
            string idsString = string.Join(", ", missingIds);
            string msg = string.Format(ErrorMessages.RecordsNotFound, nameof(CategoryEntity), nameof(CategoryEntity.Id), idsString);
            return Result.Fail(msg).LogTo(thisLogger).WithStatusCode(StatusCodes.Status404NotFound);
        }
        return dtos;
    }
    #endregion
}