using ErrorOr;
using Mediator;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using ToDoApp.Web.Shared.Extensions;
using ToDoApp.Web.Features.Tasks.Dtos;
using ToDoApp.Web.Features.Tasks_Categories;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace ToDoApp.Web.Features.Tasks.Handlers.CreateTask;

public static class CreateTaskEndpoint
{
    public const string Url = "/api/tasks";

    public static async Task<IResult> CreateTask(
        [FromBody] TaskDto dto,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken
    )
    {
        ErrorOr<CreateTaskResponse> response = await mediator.Send(new CreateTaskCommand(dto), cancellationToken);
        return response.Then(value => value.CreatedId).ToHttpResult();
    }

    extension(RouteHandlerBuilder thisBuilder)
    {
        public RouteHandlerBuilder AddCreateTaskProductionProblems()
        {
            thisBuilder.ProducesProblem(StatusCodes.Status401Unauthorized);
            thisBuilder.ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity);
            return thisBuilder.AddTask_Category_UpdateProductionProblems();
        }
    }
    extension(WebApplication thisApp)
    {
        public void AddCreateTaskEndpoint()
        {
            thisApp.MapPost(Url, CreateTask).RequireAuthorization()
                .WithName(nameof(CreateTask))
                .Produces<Guid>(StatusCodes.Status200OK)
                .AddCreateTaskProductionProblems();
        }
    }
    extension(HttpClient thisHttpClient)
    {
        public async ValueTask<HttpResponseMessage> SendCreateTaskAsync(string accessToken, TaskDto dto, CancellationToken cancellationToken)
        {
            using HttpRequestMessage request = new(HttpMethod.Post, Url);
            request.Headers.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, accessToken);
            request.Content = JsonContent.Create(dto);
            return await thisHttpClient.SendAsync(request, cancellationToken);
        }
    }
}