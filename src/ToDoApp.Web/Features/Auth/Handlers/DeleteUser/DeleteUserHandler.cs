using ErrorOr;
using Mediator;
using ToDoApp.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Data.Features.Auth.Users;
using ToDoApp.Web.Shared.Behaviors.Authorized;
using ToDoApp.Data.Features.Auth.RefreshTokens;
using ToDoApp.Web.Shared.Behaviors.DbTransaction;
using ToDoApp.Web.Features.Tasks.Handlers.DeleteTasks;
using ToDoApp.Web.Features.Categories.Handlers.DeleteCategories;

namespace ToDoApp.Web.Features.Auth.Handlers.DeleteUser;

public sealed class DeleteUserHandler(
    AppDbContext thisDbContext,
    UserManager<UserEntity> thisUserManager,
    IMediator thisMediator
) : ICommandHandler<DeleteUserCommand, ErrorOr<Unit>>
{
    #region Interfaces
    public async ValueTask<ErrorOr<Unit>> Handle(DeleteUserCommand command, CancellationToken cancellationToken)
    {
        UserEntity user = command.User;
        RefreshTokenEntity[] tokens = await thisDbContext.RefreshTokens.Where(e => e.UserId == user.Id).ToArrayAsync(cancellationToken);
        if(tokens.Length > 0)
        {
            thisDbContext.RefreshTokens.RemoveRange(tokens);
        }

        ErrorOr<Unit> errorOrUnit = await thisMediator.Send(new DeleteCategoriesCommand([])
        {
            BeginDbTransaction = false
        }, cancellationToken);
        if(errorOrUnit.IsError)
        {
            return errorOrUnit;
        }

        errorOrUnit = await thisMediator.Send(new DeleteTasksCommand([])
        {
            BeginDbTransaction = false
        }, cancellationToken);
        if(errorOrUnit.IsError)
        {
            return errorOrUnit;
        }

        await thisUserManager.DeleteAsync(command.User);
        return Unit.Value;
    }
    #endregion
}