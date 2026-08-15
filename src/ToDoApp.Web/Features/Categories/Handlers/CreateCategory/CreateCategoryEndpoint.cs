using ErrorOr;
using Mediator;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using ToDoApp.Web.Shared.Extensions;
using ToDoApp.Web.Features.Categories.Dtos;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace ToDoApp.Web.Features.Categories.Handlers.CreateCategory;

public static class CreateCategoryEndpoint
{
    public const string Url = "/categories";

    public static async Task<IResult> CreateCategory(
        [FromBody] CategoryDto dto,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken
    )
    {
        ErrorOr<CreateCategoryResponse> response = await mediator.Send(new CreateCategoryCommand(dto), cancellationToken);
        return response.Then(value => value.CreatedId).ToHttpResult();
    }

    extension(WebApplication thisApp)
    {
        public void AddCreateCategoryEndpoint()
        {
            thisApp.MapPost(Url, CreateCategory).RequireAuthorization()
                .WithName(nameof(CreateCategory))
                .Produces<Guid>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status401Unauthorized)
                .ProducesProblem(StatusCodes.Status403Forbidden)
                .ProducesProblem(StatusCodes.Status404NotFound)
                .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity);
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