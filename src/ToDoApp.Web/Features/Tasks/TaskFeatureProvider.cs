//using ErrorOr;
//using Mediator;
//using Microsoft.AspNetCore.Mvc;
//using ToDoApp.Web.Shared.Extensions;
//using ToDoApp.Web.Features.Tasks.Dtos;
//using ToDoApp.Web.Features.Tasks.Handlers;

//namespace ToDoApp.Web.Features.Tasks;

//public sealed class TaskFeatureProvider : IFeatureProvider
//{
//    #region Static
//    public static async Task<IResult> GetTasks(
//        [FromQuery] Guid[] ids,
//        [FromServices] IMediator mediator,
//        CancellationToken cancellationToken
//    )
//    {
//        ErrorOr<GetTasksResponse> response = await mediator.Send(new GetTasksQuery(ids), cancellationToken);
//        return response.Then(value => value.Tasks).ToHttpResult();
//    }
//    public static async Task<IResult> GetTasksByFilter(
//        [FromQuery] string? search,
//        [FromQuery] string? category,
//        [FromQuery] bool? done,
//        [FromQuery] int? skip,
//        [FromQuery] int? take,
//        [FromServices] IMediator mediator,
//        CancellationToken cancellationToken
//    )
//    {
//        GetTasksByFilterQuery query = new(search, category, done, skip, take);
//        ErrorOr<GetTasksByFilterResponse> response = await mediator.Send(query, cancellationToken);
//        return response.Then(value => value.Tasks).ToHttpResult();
//    }
//    public static async Task<IResult> CreateTask(
//        [FromBody] TaskDto dto,
//        [FromServices] IMediator mediator,
//        CancellationToken cancellationToken
//    )
//    {
//        ErrorOr<CreateTaskResponse> response = await mediator.Send(new CreateTaskCommand(dto), cancellationToken);
//        return response.Then(value => value.CreatedId).ToHttpResult();
//    }
//    public static async Task<IResult> UpdateTask(
//        [FromBody] TaskDto dto,
//        [FromServices] IMediator mediator,
//        CancellationToken cancellationToken
//    )
//    {
//        ErrorOr<Unit> response = await mediator.Send(new UpdateTaskCommand(dto), cancellationToken);
//        return response.ToHttpResult();
//    }
//    public static async Task<IResult> DeleteTasks(
//        [FromQuery] Guid[] ids,
//        [FromServices] IMediator mediator,
//        CancellationToken cancellationToken
//    )
//    {
//        ErrorOr<Unit> response = await mediator.Send(new DeleteTasksCommand(ids), cancellationToken);
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
//        RouteGroupBuilder group = builder.MapGroup("/task");
//        group.MapGet("/", GetTasks).RequireAuthorization()
//            .WithName(nameof(GetTasks))
//            .Produces<IEnumerable<TaskDto>>(StatusCodes.Status200OK)
//            .ProducesProblem(StatusCodes.Status401Unauthorized)
//            .ProducesProblem(StatusCodes.Status404NotFound)
//            .ProducesProblem(StatusCodes.Status500InternalServerError);
//        group.MapGet("/filter", GetTasksByFilter).RequireAuthorization()
//            .WithName(nameof(GetTasksByFilter))
//            .Produces<IEnumerable<TaskDto>>(StatusCodes.Status200OK)
//            .ProducesProblem(StatusCodes.Status401Unauthorized)
//            .ProducesProblem(StatusCodes.Status500InternalServerError)
//            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity);
//        group.MapPost("/", CreateTask).RequireAuthorization()
//            .WithName(nameof(CreateTask))
//            .Produces<Guid>(StatusCodes.Status200OK)
//            .ProducesProblem(StatusCodes.Status401Unauthorized)
//            .ProducesProblem(StatusCodes.Status404NotFound)
//            .ProducesProblem(StatusCodes.Status500InternalServerError)
//            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity);
//        group.MapPut("/", UpdateTask).RequireAuthorization()
//            .WithName(nameof(UpdateTask))
//            .Produces(StatusCodes.Status204NoContent)
//            .ProducesProblem(StatusCodes.Status401Unauthorized)
//            .ProducesProblem(StatusCodes.Status404NotFound)
//            .ProducesProblem(StatusCodes.Status500InternalServerError)
//            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity);
//        group.MapDelete("/", DeleteTasks).RequireAuthorization()
//            .WithName(nameof(DeleteTasks))
//            .Produces(StatusCodes.Status204NoContent)
//            .ProducesProblem(StatusCodes.Status401Unauthorized)
//            .ProducesProblem(StatusCodes.Status404NotFound)
//            .ProducesProblem(StatusCodes.Status500InternalServerError);
//    }
//    #endregion
//}