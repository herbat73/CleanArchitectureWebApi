using System.Reflection;
using CleanArchitectureWebApi.Application.Common.Interfaces;
using CleanArchitectureWebApi.Application.Common.Middleware;
using CleanArchitectureWebApi.Application.Common.Validation;
using Microsoft.Extensions.Hosting;
using Wolverine;
using Wolverine.FluentValidation;
using Wolverine.Runtime.Handlers;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        builder.Services.AddSingleton(typeof(IFailureAction<>), typeof(ValidationFailureAction<>));

        builder.Services.AddWolverine(opts =>
        {
            opts.Discovery.IncludeAssembly(Assembly.GetExecutingAssembly());

            // These registrations are opaque lambda factories that Wolverine cannot inline:
            // - IApplicationDbContext maps to the scoped ApplicationDbContext via a lambda (same instance per scope).
            // - IIdentityService depends on UserManager, which depends on EF Core's opaque DbContextOptions<T> factory.
            // Opt in just these types; everything else is constructor-injected by generated code.
            opts.CodeGeneration.AlwaysUseServiceLocationFor<IApplicationDbContext>();
            opts.CodeGeneration.AlwaysUseServiceLocationFor<IIdentityService>();

            // Only apply the application pipeline to application requests, not domain events.
            static bool IsApplicationRequest(HandlerChain chain) =>
                chain.MessageType.Namespace?.StartsWith("CleanArchitectureWebApi.Application", StringComparison.Ordinal) == true;

            opts.Policies.AddMiddleware<LoggingMiddleware>(IsApplicationRequest);
            opts.Policies.AddMiddleware<AuthorizationMiddleware>(IsApplicationRequest);
            opts.UseFluentValidation(RegistrationBehavior.ExplicitRegistration);
            opts.Policies.AddMiddleware<PerformanceMiddleware>(IsApplicationRequest);
        });
    }
}
