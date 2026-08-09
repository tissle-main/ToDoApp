using ErrorOr;
using Mediator;
using ToDoApp.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Data.Features.Auth.Users;
using ToDoApp.Data.Features.Auth.RefreshTokens;

namespace ToDoApp.Web.Features.Auth.Handlers;

public sealed class DeleteUserHandler(
    AppDbContext thisDbContext,
    UserManager<UserEntity> thisUserManager
) : ICommandHandler<DeleteUserCommand, ErrorOr<Unit>>
{
    #region Interfaces
    public async ValueTask<ErrorOr<Unit>> Handle(DeleteUserCommand command, CancellationToken cancellationToken)
    {
        RefreshTokenEntity[] tokens = await thisDbContext.RefreshTokens.AsNoTracking().Where(e => e.UserId == command.User.Id).ToArrayAsync(cancellationToken);
        if(tokens.Length > 0)
        {
            thisDbContext.RefreshTokens.RemoveRange(tokens);
        }

        await thisDbContext.SaveChangesAsync(cancellationToken);
        await thisUserManager.DeleteAsync(command.User);
        return Unit.Value;
    }
    #endregion
}