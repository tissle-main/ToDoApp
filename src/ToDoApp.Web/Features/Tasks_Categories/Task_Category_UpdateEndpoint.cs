using ErrorOr;
using Mediator;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using ToDoApp.Web.Shared.Extensions;
using ToDoApp.Web.Shared.JoinEntities;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace ToDoApp.Web.Features.Tasks_Categories;

public static class Task_Category_UpdateEndpoint
{
    public const string Url = "/task-category-update";

    public static async Task<IResult> Task_Category_Update(
        [FromBody] Task_Category_UpdateCommand command,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken
    )
    {
        ErrorOr<Unit> response = await mediator.Send(command, cancellationToken);
        return response.ToHttpResult();
    }

    extension(RouteHandlerBuilder thisBuilder)
    {
        public RouteHandlerBuilder AddTask_Category_UpdateProductionProblems()
        {
            return thisBuilder.AddUpdateJoinEntitiesProductionProblems();
        }
    }
    extension(WebApplication thisApp)
    {
        public void AddTask_Category_UpdateEndpoint()
        {
            thisApp.MapPost(Url, Task_Category_Update).RequireAuthorization()
                .WithName(nameof(Task_Category_Update))
                .Produces(StatusCodes.Status204NoContent)
                .AddTask_Category_UpdateProductionProblems();
        }
    }
    extension(HttpClient thisHttpClient)
    {
        public async ValueTask<HttpResponseMessage> SendTask_Category_UpdateAsync(
            string accessToken,
            Task_Category_UpdateCommand command,
            CancellationToken cancellationToken
        )
        {
            using HttpRequestMessage request = new(HttpMethod.Post, Url);
            request.Headers.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, accessToken);
            request.Content = JsonContent.Create(command);
            return await thisHttpClient.SendAsync(request, cancellationToken);
        }
    }
}