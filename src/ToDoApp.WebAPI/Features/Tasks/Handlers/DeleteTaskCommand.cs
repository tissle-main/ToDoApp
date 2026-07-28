using Mediator;
using FluentResults;

namespace ToDoApp.WebAPI.Features.Tasks.Handlers;

public sealed record class DeleteTaskCommand(Guid Id) : IRequest<Result>;