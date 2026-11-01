using CleanArchitectureWebApi.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using Wolverine;

namespace CleanArchitectureWebApi.Application.Common.Middleware;

public class LoggingMiddleware
{
    public static async Task Before(
        Envelope envelope,
        IUser user,
        IIdentityService identityService,
        ILoggerFactory loggerFactory)
    {
        var message = envelope.Message!;
        var logger = loggerFactory.CreateLogger(message.GetType());
        var requestName = message.GetType().Name;
        var userId = user.Id ?? string.Empty;
        string? userName = string.Empty;

        if (!string.IsNullOrEmpty(userId))
        {
            userName = await identityService.GetUserNameAsync(userId);
        }

        logger.LogInformation("CleanArchitectureWebApi Request: {Name} {@UserId} {@UserName} {@Request}",
            requestName, userId, userName, message);
    }
}
