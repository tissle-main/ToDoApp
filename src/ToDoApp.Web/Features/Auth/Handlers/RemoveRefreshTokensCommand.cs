using ErrorOr;
using Mediator;
using ToDoApp.Data.Features.Auth.Users;

namespace ToDoApp.Web.Features.Auth.Handlers;

public sealed record class RemoveRefreshTokensCommand(UserEntity User) : ICommand<ErrorOr<Unit>>;