using CleanArchietureNoFrontEnd.Application.Common.Interfaces;
using CleanArchietureNoFrontEnd.Domain.Entities;

namespace CleanArchietureNoFrontEnd.Application.TodoItems.Commands.CreateTodoItem;

public record CreateTodoItemCommand
{
    public int ListId { get; init; }

    public string? Title { get; init; }
}

public class CreateTodoItemCommandHandler
{
    private readonly IApplicationDbContext _context;

    public CreateTodoItemCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreateTodoItemCommand request, CancellationToken cancellationToken)
    {
        var entity = new TodoItem
        {
            ListId = request.ListId,
            Title = request.Title,
            Done = false
        };

        _context.TodoItems.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
