using Microsoft.OpenApi;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.AspNetCore.Authorization;

namespace ToDoApp.Web.Shared.Scalar;

public sealed class BearerSecurityOperationTransformer : IOpenApiOperationTransformer
{
    #region Interfaces
    public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
    {
        IList<object> metadata = context.Description.ActionDescriptor.EndpointMetadata;
        bool hasAuthorize = metadata.OfType<AuthorizeAttribute>().Any();
        bool hasAllowAnonymous = metadata.OfType<AllowAnonymousAttribute>().Any();
        if(!hasAuthorize || hasAllowAnonymous)
        {
            return Task.CompletedTask;
        }
        operation.Security ??= [];
        operation.Security.Add(new OpenApiSecurityRequirement()
        {
            [new OpenApiSecuritySchemeReference("Bearer", context.Document)] = []
        });
        return Task.CompletedTask;
    }
    #endregion
}