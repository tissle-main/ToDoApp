using ErrorOr;
using Mediator;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using ToDoApp.Web.Shared.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace ToDoApp.Web.Features.Auth.Handlers.DeleteUser;

public static class DeleteUserEndpoint
{
    public const string Url = "/auth/delete";

    public static async Task<IResult> DeleteUser(
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken
    )
    {
        ErrorOr<Unit> result = await mediator.Send(new DeleteUserCommand(), cancellationToken);
        return result.ToHttpResult();
    }

    extension(IEndpointRouteBuilder thisBuilder)
    {
        public void AddDeleteUserEndpoint()
        {
            thisBuilder.MapDelete(Url, DeleteUser).RequireAuthorization()
                .WithName(nameof(DeleteUser))
                .Produces(StatusCodes.Status204NoContent)
                .ProducesProblem(StatusCodes.Status401Unauthorized);
        }        
    }
    extension(HttpClient thisHttpClient)
    {
        public async ValueTask<HttpResponseMessage> SendDeleteUserAsync(string accessToken, CancellationToken cancellationToken)
        {
            using HttpRequestMessage request = new(HttpMethod.Delete, Url);
            request.Headers.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, accessToken);
            return await thisHttpClient.SendAsync(request, cancellationToken);
        }
    }
}