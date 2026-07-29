using Mediator;
using FluentResults;
using Microsoft.AspNetCore.Mvc;
using ToDoApp.WebAPI.Extensions;
using ToDoApp.WebAPI.Features.Tasks.Dtos;
using ToDoApp.WebAPI.Features.Tasks.Handlers;

namespace ToDoApp.WebAPI.Features.Tasks;

[ApiController]
[Route("/api")]
public sealed class TaskController : ControllerBase
{
    [HttpGet("/task", Name = nameof(GetTasks))]
    [ProducesResponseType<TaskDto[]>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTasks(
        [FromQuery] Guid[] ids,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken
    )
    {
        Result<TaskDto[]> result = await mediator.Send(new GetTasksQuery(ids), cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet("/task/filter", Name = nameof(GetTasksByFilter))]
    [ProducesResponseType<TaskDto[]>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<HttpValidationProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> GetTasksByFilter(
        [FromQuery] GetTasksByFilterQuery query,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken
    )
    {
        Result<TaskDto[]> result = await mediator.Send(query, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost("/task", Name = nameof(CreateTask))]
    [ProducesResponseType<Guid>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<HttpValidationProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> CreateTask(
        [FromBody] TaskDto dto,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken
    )
    {
        Result<Guid> result = await mediator.Send(new CreateTaskCommand(dto), cancellationToken);
        return result.ToActionResult();
    }

    [HttpPut("/task", Name = nameof(UpdateTask))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<HttpValidationProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> UpdateTask(
        [FromBody] TaskDto dto,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken
    )
    {
        Result result = await mediator.Send(new UpdateTaskCommand(dto), cancellationToken);
        return result.ToActionResult();
    }

    [HttpDelete("/task", Name = nameof(DeleteTasks))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteTasks(
        [FromQuery] Guid[] ids,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken
    )
    {
        Result result = await mediator.Send(new DeleteTasksCommand(ids), cancellationToken);
        return result.ToActionResult();
    }
}