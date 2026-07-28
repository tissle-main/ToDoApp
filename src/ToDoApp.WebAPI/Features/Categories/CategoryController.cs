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
    [HttpGet("/category", Name = nameof(GetAllCategories))]
    [ProducesResponseType<IEnumerable<CategoryDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllCategories(
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken
    )
    {
        Result<IEnumerable<CategoryDto>> result = await mediator.Send(new GetAllCategoriesQuery(), cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet("/category/{id:guid}", Name = nameof(GetCategoryById))]
    [ProducesResponseType<CategoryDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCategoryById(
        [FromRoute] Guid id,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken
    )
    {
        Result<CategoryDto> result = await mediator.Send(new GetCategoryByIdQuery(id), cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost("/category", Name = nameof(CreateCategory))]
    [ProducesResponseType<Guid>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
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

    [HttpDelete("/category/{id:guid}", Name = nameof(DeleteCategory))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCategory(
        [FromRoute] Guid id,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken
    )
    {
        Result result = await mediator.Send(new DeleteCategoryCommand(id), cancellationToken);
        return result.ToActionResult();
    }
}