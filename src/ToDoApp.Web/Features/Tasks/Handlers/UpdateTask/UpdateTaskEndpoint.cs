using ErrorOr;
using Mediator;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using ToDoApp.Web.Shared.Extensions;
using ToDoApp.Web.Features.Tasks.Dtos;
using ToDoApp.Web.Features.Tasks_Categories;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace ToDoApp.Web.Features.Tasks.Handlers.UpdateTask;

public static class UpdateTaskEndpoint
{
    public const string Url = "/tasks";

    public static async Task<IResult> UpdateTask(
        [FromBody] TaskDto dto,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken
    )
    {
        ErrorOr<Unit> response = await mediator.Send(new UpdateTaskCommand(dto), cancellationToken);
        return response.ToHttpResult();
    }

    extension(RouteHandlerBuilder thisBuilder)
    {
        public RouteHandlerBuilder AddUpdateTaskProductionProblems()
        {
            thisBuilder.ProducesProblem(StatusCodes.Status401Unauthorized);
            thisBuilder.ProducesProblem(StatusCodes.Status404NotFound);
            thisBuilder.ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity);
            return thisBuilder.AddTask_Category_UpdateProductionProblems();
        }
    }
    extension(WebApplication thisApp)
    {
        public void AddUpdateTaskEndpoint()
        {
            thisApp.MapPut(Url, UpdateTask).RequireAuthorization()
                .WithName(nameof(UpdateTask))
                .Produces(StatusCodes.Status204NoContent)
                .AddUpdateTaskProductionProblems();
        }
    }
    extension(HttpClient thisHttpClient)
    {
        public async ValueTask<HttpResponseMessage> SendUpdateTaskAsync(string accessToken, TaskDto dto, CancellationToken cancellationToken)
        {
            using HttpRequestMessage request = new(HttpMethod.Put, Url);
            request.Headers.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, accessToken);
            request.Content = JsonContent.Create(dto);
            return await thisHttpClient.SendAsync(request, cancellationToken);
        }
    }
}