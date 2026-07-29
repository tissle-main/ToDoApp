using Mediator;
using FluentResults;

namespace ToDoApp.WebAPI.Features.Tasks.Handlers;

public sealed record class DeleteTasksCommand(Guid[] Ids) : IRequest<Result>;