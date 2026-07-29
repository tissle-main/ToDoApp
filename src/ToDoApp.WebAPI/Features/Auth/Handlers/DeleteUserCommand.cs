using Mediator;
using FluentResults;

namespace ToDoApp.WebAPI.Features.Auth.Handlers;

public sealed record class DeleteUserCommand() : IRequest<Result>;