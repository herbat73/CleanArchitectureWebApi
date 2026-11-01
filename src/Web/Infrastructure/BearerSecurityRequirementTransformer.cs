using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace CleanArchietureNoFrontEnd.Web.Infrastructure;

/// <summary>
/// Marks every operation that requires authorization with the <c>Bearer</c> security requirement,
/// so the Scalar UI shows the auth input and sends <c>Authorization: Bearer &lt;token&gt;</c>
/// for protected endpoints. Anonymous endpoints (<see cref="IAllowAnonymous"/>) are left unchanged.
/// </summary>
internal sealed class BearerSecurityRequirementTransformer : IOpenApiOperationTransformer
{
    public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
    {
        var metadata = context.Description.ActionDescriptor.EndpointMetadata;

        var requiresAuth = metadata.Any(m => m is IAuthorizeData)
            && !metadata.Any(m => m is IAllowAnonymous);

        if (requiresAuth)
        {
            operation.Security ??= [];
            operation.Security.Add(new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("Bearer", context.Document)] = []
            });
        }

        return Task.CompletedTask;
    }
}
