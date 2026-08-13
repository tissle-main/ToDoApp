using ErrorOr;
using Mediator;
using ToDoApp.Data;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using ToDoApp.Data.Features.Auth;
using ToDoApp.Web.Shared.Behaviors;
using ToDoApp.Web.Shared.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Web.Features.Tasks.Handlers;
using ToDoApp.Web.Features.Categories.Handlers;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace ToDoApp.Web.Features.Auth.Handlers;

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
        RefreshTokenEntity[] tokens = await thisDbContext.RefreshTokens.AsNoTracking().Where(e => e.UserId == user.Id).ToArrayAsync(cancellationToken);
        if(tokens.Length > 0)
        {
            thisDbContext.RefreshTokens.RemoveRange(tokens);
        }

        ErrorOr<Unit> errorOrUnit = await thisMediator.Send(new DeleteCategoriesCommand([])
        {
            SaveDatabase = false
        }, cancellationToken);
        if(errorOrUnit.IsError)
        {
            return errorOrUnit;
        }

        errorOrUnit = await thisMediator.Send(new DeleteTasksCommand([])
        {
            SaveDatabase = false
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
public sealed record class DeleteUserCommand : IAuthorizedMessage, ICommand<ErrorOr<Unit>>;
public static class DeleteUserEndpoint
{
    public const string Url = "/auth/delete";

    public static async Task<IResult> DeleteUser(
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken
    )
    {
        ErrorOr<Unit> result = await mediator.Send(new DeleteUserCommand(), cancellationToken);
        return result.ToHttpResult();
    }
    public static void AddDeleteUserEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapDelete(Url, DeleteUser).RequireAuthorization()
            .WithName(nameof(DeleteUser))
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
    public static async ValueTask<HttpResponseMessage> SendDeleteUserAsync(
        this HttpClient httpClient,
        string accessToken,
        CancellationToken cancellationToken
    )
    {
        using HttpRequestMessage request = new(HttpMethod.Delete, Url);
        request.Headers.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, accessToken);
        return await httpClient.SendAsync(request, cancellationToken);
    }
}