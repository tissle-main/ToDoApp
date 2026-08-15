using ErrorOr;
using Mediator;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using ToDoApp.Web.Shared.Extensions;
using Microsoft.AspNetCore.WebUtilities;
using ToDoApp.Web.Features.Categories.Dtos;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace ToDoApp.Web.Features.Categories.Handlers.GetCategories;

public static class GetCategoriesEndpoint
{
    public const string Url = "/categories";

    public static string CreateSendableUrl(Guid[] ids)
    {
        IEnumerable<KeyValuePair<string, string?>> queryParams = ids.Select(id =>
        {
            return new KeyValuePair<string, string?>(nameof(ids), id.ToString());
        });
        return QueryHelpers.AddQueryString(Url, queryParams);
    }
    public static async Task<IResult> GetCategories(
        [FromQuery] Guid[] ids,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken
    )
    {
        ErrorOr<GetCategoriesResponse> response = await mediator.Send(new GetCategoriesQuery(ids), cancellationToken);
        return response.Then(value => value.Categories).ToHttpResult();
    }

    extension(WebApplication thisApp)
    {
        public void AddGetCategoriesEndpoint()
        {
            thisApp.MapGet(Url, GetCategories).RequireAuthorization()
                .WithName(nameof(GetCategories))
                .Produces<IEnumerable<CategoryDto>>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status401Unauthorized)
                .ProducesProblem(StatusCodes.Status404NotFound);
        }
    }
    extension(HttpClient thisHttpClient)
    {
        public async ValueTask<HttpResponseMessage> SendGetCategoriesAsync(string accessToken, Guid[] ids, CancellationToken cancellationToken)
        {
            using HttpRequestMessage request = new(HttpMethod.Get, CreateSendableUrl(ids));
            request.Headers.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, accessToken);
            return await thisHttpClient.SendAsync(request, cancellationToken);
        }
    }
}