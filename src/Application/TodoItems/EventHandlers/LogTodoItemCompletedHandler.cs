using CleanArchitectureWebApi.Domain.Events;
using Microsoft.Extensions.Logging;

namespace CleanArchitectureWebApi.Application.TodoItems.EventHandlers;

public class LogTodoItemCompletedHandler
{
    private readonly ILogger<LogTodoItemCompletedHandler> _logger;

    public LogTodoItemCompletedHandler(ILogger<LogTodoItemCompletedHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(TodoItemCompletedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("CleanArchitectureWebApi Domain Event: {DomainEvent}", notification.GetType().Name);

        return Task.CompletedTask;
    }
}
