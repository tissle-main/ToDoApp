using ErrorOr;
using Mediator;

namespace ToDoApp.Web.Features.Auth.Handlers;

public sealed record class LoginUserCommand(string Email, string Password) : ICommand<ErrorOr<LoginUserResponse>>;