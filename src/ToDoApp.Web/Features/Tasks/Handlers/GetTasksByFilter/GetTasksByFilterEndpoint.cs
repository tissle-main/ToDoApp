using ErrorOr;
using Mediator;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using ToDoApp.Web.Shared.Extensions;
using ToDoApp.Web.Features.Tasks.Dtos;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace ToDoApp.Web.Features.Tasks.Handlers.GetTasksByFilter;

public static class GetTasksEndpoint
{
    public const string Url = "/api/tasks/filter";

    public static string CreateSendableUrl(GetTasksByFilterQuery query)
    {
        Dictionary<string, string?> queryParams = [];
        if(query.Search is not null)
        {
            queryParams.Add(nameof(query.Search), query.Search);
        }
        if(query.Category is not null)
        {
            queryParams.Add(nameof(query.Category), query.Category);
        }
        if(query.Done is bool done)
        {
            queryParams.Add(nameof(query.Done), done.ToString());
        }
        if(query.Skip is int skip)
        {
            queryParams.Add(nameof(query.Skip), skip.ToString());
        }
        if(query.Take is int take)
        {
            queryParams.Add(nameof(query.Take), take.ToString());
        }
        return QueryHelpers.AddQueryString(Url, queryParams);
    }
    public static async Task<IResult> GetTasksByFilter(
        [FromServices] IMediator mediator,
        [FromQuery] string? search = null,
        [FromQuery] string? category = null,
        [FromQuery] bool? done = null,
        [FromQuery] int? skip = null,
        [FromQuery] int? take = null,
        CancellationToken cancellationToken = default
    )
    {
        ErrorOr<GetTasksByFilterResponse> response = await mediator.Send(new GetTasksByFilterQuery(search, category, done, skip, take), cancellationToken);
        return response.Then(value => value.Tasks).ToHttpResult();
    }

    extension(RouteHandlerBuilder thisBuilder)
    {
        public RouteHandlerBuilder AddGetTasksByFilterProductionProblems()
        {
            thisBuilder.ProducesProblem(StatusCodes.Status401Unauthorized);
            return thisBuilder.ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity);
        }
    }
    extension(WebApplication thisApp)
    {
        public void AddGetTasksByFilterEndpoint()
        {
            thisApp.MapGet(Url, GetTasksByFilter).RequireAuthorization()
                .WithName(nameof(GetTasksByFilter))
                .Produces<IEnumerable<TaskDto>>(StatusCodes.Status200OK)
                .AddGetTasksByFilterProductionProblems();
        }
    }
    extension(HttpClient thisHttpClient)
    {
        public async ValueTask<HttpResponseMessage> SendGetTasksByFilterAsync(
            string accessToken,
            GetTasksByFilterQuery query,
            CancellationToken cancellationToken
        )
        {
            using HttpRequestMessage request = new(HttpMethod.Get, CreateSendableUrl(query));
            request.Headers.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, accessToken);
            return await thisHttpClient.SendAsync(request, cancellationToken);
        }
    }
}