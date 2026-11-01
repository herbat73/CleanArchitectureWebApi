using CleanArchitectureWebApi.Application.Common.Interfaces;
using CleanArchitectureWebApi.Domain.Entities;
using CleanArchitectureWebApi.Domain.ValueObjects;

namespace CleanArchitectureWebApi.Application.TodoLists.Commands.CreateTodoList;

public record CreateTodoListCommand
{
    public string? Title { get; init; }

    public string? Colour { get; init; }
}

public class CreateTodoListCommandHandler
{
    private readonly IApplicationDbContext _context;

    public CreateTodoListCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreateTodoListCommand request, CancellationToken cancellationToken)
    {
        var entity = new TodoList
        {
            Title = request.Title,
            Colour = Colour.From(request.Colour ?? Colour.Grey)
        };

        _context.TodoLists.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
