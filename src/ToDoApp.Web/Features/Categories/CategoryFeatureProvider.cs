//using ErrorOr;
//using Mediator;
//using Microsoft.AspNetCore.Mvc;
//using ToDoApp.Web.Shared.Extensions;
//using ToDoApp.Web.Features.Tasks.Dtos;
//using ToDoApp.Web.Features.Categories.Dtos;
//using ToDoApp.Web.Features.Categories.Handlers;

//namespace ToDoApp.Web.Features.Categories;

//public sealed class CategoryFeatureProvider : IFeatureProvider
//{
//    #region Static
//    public static async Task<IResult> GetCategories(
//        [FromQuery] Guid[] ids,
//        [FromServices] IMediator mediator,
//        CancellationToken cancellationToken
//    )
//    {
//        ErrorOr<GetCategoriesResponse> response = await mediator.Send(new GetCategoriesQuery(ids), cancellationToken);
//        return response.Then(value => value.Categories).ToHttpResult();
//    }
//    public static async Task<IResult> CreateCategory(
//        [FromBody] CategoryDto dto,
//        [FromServices] IMediator mediator,
//        CancellationToken cancellationToken
//    )
//    {
//        ErrorOr<CreateCategoryResponse> response = await mediator.Send(new CreateCategoryCommand(dto), cancellationToken);
//        return response.Then(value => value.CreatedId).ToHttpResult();
//    }
//    public static async Task<IResult> UpdateCategory(
//        [FromBody] CategoryDto dto,
//        [FromServices] IMediator mediator,
//        CancellationToken cancellationToken
//    )
//    {
//        ErrorOr<Unit> response = await mediator.Send(new UpdateCategoryCommand(dto), cancellationToken);
//        return response.ToHttpResult();
//    }
//    public static async Task<IResult> DeleteCategories(
//        [FromQuery] Guid[] ids,
//        [FromServices] IMediator mediator,
//        CancellationToken cancellationToken
//    )
//    {
//        ErrorOr<Unit> response = await mediator.Send(new DeleteCategoriesCommand(ids), cancellationToken);
//        return response.ToHttpResult();
//    }
//    #endregion

//    #region Interfaces
//    public void AddServices(IHostApplicationBuilder builder)
//    {

//    }
//    public void UseMiddleware(IApplicationBuilder builder)
//    {

//    }
//    public void MapEndpoints(IEndpointRouteBuilder builder)
//    {
//        RouteGroupBuilder group = builder.MapGroup("/category");
//        group.MapGet("/", GetCategories).RequireAuthorization()
//            .WithName(nameof(GetCategories))
//            .Produces<IEnumerable<TaskDto>>(StatusCodes.Status200OK)
//            .ProducesProblem(StatusCodes.Status401Unauthorized)
//            .ProducesProblem(StatusCodes.Status404NotFound)
//            .ProducesProblem(StatusCodes.Status500InternalServerError);
//        group.MapPost("/", CreateCategory).RequireAuthorization()
//            .WithName(nameof(CreateCategory))
//            .Produces<Guid>(StatusCodes.Status200OK)
//            .ProducesProblem(StatusCodes.Status401Unauthorized)
//            .ProducesProblem(StatusCodes.Status404NotFound)
//            .ProducesProblem(StatusCodes.Status500InternalServerError)
//            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity);
//        group.MapPut("/", UpdateCategory).RequireAuthorization()
//            .WithName(nameof(UpdateCategory))
//            .Produces(StatusCodes.Status204NoContent)
//            .ProducesProblem(StatusCodes.Status401Unauthorized)
//            .ProducesProblem(StatusCodes.Status404NotFound)
//            .ProducesProblem(StatusCodes.Status500InternalServerError)
//            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity);
//        group.MapDelete("/", DeleteCategories).RequireAuthorization()
//            .WithName(nameof(DeleteCategories))
//            .Produces(StatusCodes.Status204NoContent)
//            .ProducesProblem(StatusCodes.Status401Unauthorized)
//            .ProducesProblem(StatusCodes.Status404NotFound)
//            .ProducesProblem(StatusCodes.Status500InternalServerError);
//    }
//    #endregion
//}