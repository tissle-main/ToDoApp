using ErrorOr;
using Mediator;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using ToDoApp.Web.Shared.Extensions;
using ToDoApp.Web.Features.Categories.Dtos;
using ToDoApp.Web.Features.Tasks_Categories;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace ToDoApp.Web.Features.Categories.Handlers.CreateCategory;

public static class CreateCategoryEndpoint
{
    public const string Url = "/api/categories";

    public static async Task<IResult> CreateCategory(
        [FromBody] CategoryDto dto,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken
    )
    {
        ErrorOr<CreateCategoryResponse> response = await mediator.Send(new CreateCategoryCommand(dto), cancellationToken);
        return response.Then(value => value.CreatedId).ToHttpResult();
    }

    extension(RouteHandlerBuilder thisBuilder)
    {
        public RouteHandlerBuilder AddCreateCategoryProductionProblems()
        {
            thisBuilder.ProducesProblem(StatusCodes.Status401Unauthorized);
            thisBuilder.ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity);
            return thisBuilder.AddTask_Category_UpdateProductionProblems();
        }
    }
    extension(WebApplication thisApp)
    {
        public void AddCreateCategoryEndpoint()
        {
            thisApp.MapPost(Url, CreateCategory).RequireAuthorization()
                .WithName(nameof(CreateCategory))
                .Produces<Guid>(StatusCodes.Status200OK)
                .AddCreateCategoryProductionProblems();
        }
    }  
    extension(HttpClient thisHttpClient)
    {
        public async ValueTask<HttpResponseMessage> SendCreateCategoryAsync(string accessToken, CategoryDto dto, CancellationToken cancellationToken)
        {
            using HttpRequestMessage request = new(HttpMethod.Post, Url);
            request.Headers.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, accessToken);
            request.Content = JsonContent.Create(dto);
            return await thisHttpClient.SendAsync(request, cancellationToken);
        }
    }  
}