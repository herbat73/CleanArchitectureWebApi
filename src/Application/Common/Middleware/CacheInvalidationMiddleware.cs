using CleanArchitectureWebApi.Application.Common.Caching;
using Microsoft.Extensions.Caching.Hybrid;

namespace CleanArchitectureWebApi.Application.Common.Middleware;

/// <summary>
/// Applied to every command handler. Runs only after the handler completed successfully, so a failed command
/// (validation, not found, exception) leaves the cache untouched.
/// </summary>
public class CacheInvalidationMiddleware
{
    public static async Task AfterAsync(HybridCache cache, CancellationToken cancellationToken)
    {
        await cache.RemoveByTagAsync(CacheTags.Todos, cancellationToken);
    }
}
