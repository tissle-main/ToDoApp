using ErrorOr;
using Mediator;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using ToDoApp.Web.Shared.Extensions;
using ToDoApp.Web.Features.Tasks.Dtos;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace ToDoApp.Web.Features.Tasks.Handlers.GetTasks;

public static class GetTasksEndpoint
{
    public const string Url = "/tasks";

    public static string CreateSendableUrl(Guid[] ids)
    {
        IEnumerable<KeyValuePair<string, string?>> queryParams = ids.Select(id =>
        {
            return new KeyValuePair<string, string?>(nameof(ids), id.ToString());
        });
        return QueryHelpers.AddQueryString(Url, queryParams);
    }
    public static async Task<IResult> GetTasks(
        [FromQuery] Guid[] ids,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken
    )
    {
        ErrorOr<GetTasksResponse> response = await mediator.Send(new GetTasksQuery(ids), cancellationToken);
        return response.Then(value => value.Tasks).ToHttpResult();
    }

    extension(RouteHandlerBuilder thisBuilder)
    {
        public RouteHandlerBuilder AddGetTasksProductionProblems()
        {
            thisBuilder.ProducesProblem(StatusCodes.Status401Unauthorized);
            return thisBuilder.ProducesProblem(StatusCodes.Status404NotFound);
        }
    }
    extension(WebApplication thisApp)
    {
        public void AddGetTasksEndpoint()
        {
            thisApp.MapGet(Url, GetTasks).RequireAuthorization()
                .WithName(nameof(GetTasks))
                .Produces<IEnumerable<TaskDto>>(StatusCodes.Status200OK)
                .AddGetTasksProductionProblems();
        }
    }
    extension(HttpClient thisHttpClient)
    {
        public async ValueTask<HttpResponseMessage> SendGetTasksAsync(string accessToken, Guid[] ids, CancellationToken cancellationToken)
        {
            using HttpRequestMessage request = new(HttpMethod.Get, CreateSendableUrl(ids));
            request.Headers.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, accessToken);
            return await thisHttpClient.SendAsync(request, cancellationToken);
        }
    }
}