using Mediator;
using FluentResults;

namespace ToDoApp.WebAPI.Features.Auth.Handlers;

public sealed record class LoginUserCommand(string Email, string Password) : IRequest<Result<string>>;