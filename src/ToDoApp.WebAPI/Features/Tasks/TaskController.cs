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
    [ProducesResponseType<IEnumerable<TaskDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<HttpValidationProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> GetTasks(
        [FromQuery] GetTasksQuery query,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken
    )
    {
        Result<IEnumerable<TaskDto>> result = await mediator.Send(query, cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet("/task/{id:guid}", Name = nameof(GetTaskById))]
    [ProducesResponseType<TaskDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTaskById(
        [FromRoute] Guid id,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken
    )
    {
        Result<TaskDto> result = await mediator.Send(new GetTaskByIdQuery(id), cancellationToken);
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

    [HttpDelete("/task/{id:guid}", Name = nameof(DeleteTask))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteTask(
        [FromRoute] Guid id,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken
    )
    {
        Result result = await mediator.Send(new DeleteTaskCommand(id), cancellationToken);
        return result.ToActionResult();
    }
}