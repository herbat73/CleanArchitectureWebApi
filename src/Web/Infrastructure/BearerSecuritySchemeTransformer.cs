using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace CleanArchietureNoFrontEnd.Web.Infrastructure;

/// <summary>
/// Declares the <c>Bearer</c> HTTP security scheme in the OpenAPI document so the Scalar UI can
/// show the auth input and send <c>Authorization: Bearer &lt;token&gt;</c> for protected operations.
/// The scheme is always registered because <c>Infrastructure</c> always configures bearer tokens
/// (<c>AddBearerToken(IdentityConstants.BearerScheme)</c>).
/// </summary>
internal sealed class BearerSecuritySchemeTransformer : IOpenApiDocumentTransformer
{
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();

        document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            In = ParameterLocation.Header,
            BearerFormat = "Json Web Token"
        };

        return Task.CompletedTask;
    }
}
