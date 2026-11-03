namespace CleanArchitectureWebApi.Application.Common.Caching;

/// <summary>
/// Tags attached to cached entries so they can be invalidated as a group via <c>HybridCache.RemoveByTagAsync</c>.
/// </summary>
public static class CacheTags
{
    /// <summary>Everything derived from todo lists and todo items.</summary>
    public const string Todos = "todos";
}
