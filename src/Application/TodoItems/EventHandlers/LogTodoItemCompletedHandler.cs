using CleanArchietureNoFrontEnd.Domain.Events;
using Microsoft.Extensions.Logging;

namespace CleanArchietureNoFrontEnd.Application.TodoItems.EventHandlers;

public class LogTodoItemCompletedHandler
{
    private readonly ILogger<LogTodoItemCompletedHandler> _logger;

    public LogTodoItemCompletedHandler(ILogger<LogTodoItemCompletedHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(TodoItemCompletedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("CleanArchietureNoFrontEnd Domain Event: {DomainEvent}", notification.GetType().Name);

        return Task.CompletedTask;
    }
}
