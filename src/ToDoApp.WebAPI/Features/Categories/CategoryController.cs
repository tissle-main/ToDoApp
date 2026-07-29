using Mediator;
using FluentResults;
using Microsoft.AspNetCore.Mvc;
using ToDoApp.WebAPI.Extensions;
using ToDoApp.WebAPI.Features.Categories.Dtos;
using ToDoApp.WebAPI.Features.Categories.Handlers;

namespace ToDoApp.WebAPI.Features.Categories;

[ApiController]
[Route("/api")]
public sealed class CategoryController : ControllerBase
{
    [HttpGet("/category", Name = nameof(GetCategories))]
    [ProducesResponseType<CategoryDto[]>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCategories(
        [FromQuery] Guid[] ids,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken
    )
    {
        Result<CategoryDto[]> result = await mediator.Send(new GetCategoriesQuery(ids), cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost("/category", Name = nameof(CreateCategory))]
    [ProducesResponseType<Guid>(StatusCodes.Status200OK)]
    [ProducesResponseType<HttpValidationProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> CreateCategory(
        [FromBody] CategoryDto dto,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken
    )
    {
        Result<Guid> result = await mediator.Send(new CreateCategoryCommand(dto), cancellationToken);
        return result.ToActionResult();
    }

    [HttpPut("/category", Name = nameof(UpdateCategory))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<HttpValidationProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> UpdateCategory(
        [FromBody] CategoryDto dto,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken
    )
    {
        Result result = await mediator.Send(new UpdateCategoryCommand(dto), cancellationToken);
        return result.ToActionResult();
    }

    [HttpDelete("/category", Name = nameof(DeleteCategories))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCategories(
        [FromQuery] Guid[] ids,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken
    )
    {
        Result result = await mediator.Send(new DeleteCategoriesCommand(ids), cancellationToken);
        return result.ToActionResult();
    }
}