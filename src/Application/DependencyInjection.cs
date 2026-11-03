using System.Reflection;
using CleanArchitectureWebApi.Application.Common.Interfaces;
using CleanArchitectureWebApi.Application.Common.Middleware;
using CleanArchitectureWebApi.Application.Common.Validation;
using Microsoft.Extensions.Caching.Hybrid;
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

        // L1 in-memory cache with stampede protection and tag-based invalidation. Register an IDistributedCache
        // (e.g. Redis) to get a shared L2 cache across instances without any other code change.
        builder.Services.AddHybridCache(options =>
        {
            options.DefaultEntryOptions = new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromMinutes(5),
                LocalCacheExpiration = TimeSpan.FromMinutes(5)
            };
        });

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

            // Commands change data that cached queries are built from, so drop the cache once they succeed.
            opts.Policies.AddMiddleware<CacheInvalidationMiddleware>(chain =>
                IsApplicationRequest(chain) && chain.MessageType.Namespace!.Contains(".Commands.", StringComparison.Ordinal));
        });
    }
}
