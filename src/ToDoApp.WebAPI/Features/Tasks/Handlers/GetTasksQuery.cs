using Mediator;
using FluentResults;
using ToDoApp.WebAPI.Features.Tasks.Dtos;

namespace ToDoApp.WebAPI.Features.Tasks.Handlers;

public sealed record class GetTasksQuery(
    Guid? CategoryId = null,
    string? Search = null,
    bool? Done = null,
    int? From = null,
    int? Count = null
) : IRequest<Result<IEnumerable<TaskDto>>>;