using CleanArchitectureWebApi.Application.Common.Caching;
using CleanArchitectureWebApi.Application.Common.Interfaces;
using CleanArchitectureWebApi.Application.Common.Models;
using CleanArchitectureWebApi.Application.Common.Security;
using CleanArchitectureWebApi.Domain.Enums;
using CleanArchitectureWebApi.Domain.ValueObjects;
using Microsoft.Extensions.Caching.Hybrid;

namespace CleanArchitectureWebApi.Application.TodoLists.Queries.GetTodos;

[Authorize]
public record GetTodosQuery;

public class GetTodosQueryHandler
{
    private const string CacheKey = "todos:all";

    private readonly IApplicationDbContext _context;
    private readonly HybridCache _cache;

    public GetTodosQueryHandler(IApplicationDbContext context, HybridCache cache)
    {
        _context = context;
        _cache = cache;
    }

    // The result is not user-specific, so a single shared entry is used. It is evicted by CacheInvalidationMiddleware
    // whenever a todo command succeeds.
    public async Task<TodosVm> Handle(GetTodosQuery request, CancellationToken cancellationToken)
    {
        return await _cache.GetOrCreateAsync(
            CacheKey,
            async ct => await LoadAsync(ct),
            tags: [CacheTags.Todos],
            cancellationToken: cancellationToken);
    }

    private async Task<TodosVm> LoadAsync(CancellationToken cancellationToken)
    {
        return new TodosVm
        {
            PriorityLevels = Enum.GetValues(typeof(PriorityLevel))
                .Cast<PriorityLevel>()
                .Select(p => new LookupDto { Id = (int)p, Title = p.ToString() })
                .ToList(),

            Colours =
            [
                new ColourDto { Code = Colour.Grey, Name = nameof(Colour.Grey) },
                new ColourDto { Code = Colour.Purple, Name = nameof(Colour.Purple) },
                new ColourDto { Code = Colour.Blue, Name = nameof(Colour.Blue) },
                new ColourDto { Code = Colour.Teal, Name = nameof(Colour.Teal) },
                new ColourDto { Code = Colour.Green, Name = nameof(Colour.Green) },
                new ColourDto { Code = Colour.Orange, Name = nameof(Colour.Orange) },
                new ColourDto { Code = Colour.Red, Name = nameof(Colour.Red) },
            ],

            Lists = await _context.TodoLists
                .AsNoTracking()
                .Select(t => new TodoListDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Colour = t.Colour.Code,
                    Items = t.Items
                        .Select(i => new TodoItemDto
                        {
                            Id = i.Id,
                            ListId = i.ListId,
                            Title = i.Title,
                            Done = i.Done,
                            Priority = (int)i.Priority,
                            Note = i.Note
                        })
                        .ToList()
                })
                .OrderBy(t => t.Title)
                .ToListAsync(cancellationToken)
        };
    }
}
