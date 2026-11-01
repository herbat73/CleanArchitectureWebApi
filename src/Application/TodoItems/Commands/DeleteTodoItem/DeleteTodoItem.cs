using CleanArchitectureWebApi.Application.Common.Interfaces;

namespace CleanArchitectureWebApi.Application.TodoItems.Commands.DeleteTodoItem;

public record DeleteTodoItemCommand(int Id);

public class DeleteTodoItemCommandHandler
{
    private readonly IApplicationDbContext _context;

    public DeleteTodoItemCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteTodoItemCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.TodoItems
            .FindAsync([request.Id], cancellationToken);

        Guard.Against.NotFound(request.Id, entity);

        _context.TodoItems.Remove(entity);

        await _context.SaveChangesAsync(cancellationToken);
    }

}
