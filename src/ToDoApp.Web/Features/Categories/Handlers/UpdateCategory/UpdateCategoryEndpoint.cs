using ErrorOr;
using Mediator;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using ToDoApp.Web.Shared.Extensions;
using ToDoApp.Web.Features.Categories.Dtos;
using ToDoApp.Web.Features.Tasks_Categories;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace ToDoApp.Web.Features.Categories.Handlers.UpdateCategory;

public static class UpdateCategoryEndpoint
{
    public const string Url = "/api/categories";

    public static async Task<IResult> UpdateCategory(
        [FromBody] CategoryDto dto,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken
    )
    {
        ErrorOr<Unit> response = await mediator.Send(new UpdateCategoryCommand(dto), cancellationToken);
        return response.ToHttpResult();
    }

    extension(RouteHandlerBuilder thisBuilder)
    {
        public RouteHandlerBuilder AddUpdateCategoryProductionProblems()
        {
            thisBuilder.ProducesProblem(StatusCodes.Status401Unauthorized);
            thisBuilder.ProducesProblem(StatusCodes.Status404NotFound);
            thisBuilder.ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity);
            return thisBuilder.AddTask_Category_UpdateProductionProblems();
        }
    }
    extension(WebApplication thisApp)
    {
        public void AddUpdateCategoryEndpoint()
        {
            thisApp.MapPut(Url, UpdateCategory).RequireAuthorization()
                .WithName(nameof(UpdateCategory))
                .Produces(StatusCodes.Status204NoContent)
                .AddUpdateCategoryProductionProblems();
        }
    }
    extension(HttpClient thisHttpClient)
    {
        public async ValueTask<HttpResponseMessage> SendUpdateCategoryAsync(string accessToken, CategoryDto dto, CancellationToken cancellationToken)
        {
            using HttpRequestMessage request = new(HttpMethod.Put, Url);
            request.Headers.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, accessToken);
            request.Content = JsonContent.Create(dto);
            return await thisHttpClient.SendAsync(request, cancellationToken);
        }
    }
}