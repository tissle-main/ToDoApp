using Mediator;
using FluentResults;
using ToDoApp.WebAPI.Features.Tasks.Dtos;

namespace ToDoApp.WebAPI.Features.Tasks.Handlers;

public sealed record class GetTasksQuery(Guid[] Ids) : IRequest<Result<TaskDto[]>>;