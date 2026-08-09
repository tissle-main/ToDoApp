using ErrorOr;
using Mediator;

namespace ToDoApp.Web.Features.Auth.Handlers;

public sealed record class RegisterUserCommand(string Email, string Password) : ICommand<ErrorOr<Unit>>;