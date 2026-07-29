using Mediator;
using FluentResults;

namespace ToDoApp.WebAPI.Features.Auth.Handlers;

public sealed record class RegisterUserCommand(string Email, string Password) : IRequest<Result>;