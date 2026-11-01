using System.Diagnostics;
using CleanArchitectureWebApi.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using Wolverine;

namespace CleanArchitectureWebApi.Application.Common.Middleware;

public class PerformanceMiddleware
{
    public static Stopwatch Before() => Stopwatch.StartNew();

    public static async Task FinallyAsync(
        Stopwatch timer,
        Envelope envelope,
        IUser user,
        IIdentityService identityService,
        ILoggerFactory loggerFactory)
    {
        timer.Stop();

        var elapsedMilliseconds = timer.ElapsedMilliseconds;

        if (elapsedMilliseconds > 500)
        {
            var message = envelope.Message!;
            var logger = loggerFactory.CreateLogger(message.GetType());
            var requestName = message.GetType().Name;
            var userId = user.Id ?? string.Empty;
            var userName = string.Empty;

            if (!string.IsNullOrEmpty(userId))
            {
                userName = await identityService.GetUserNameAsync(userId);
            }

            logger.LogWarning("CleanArchitectureWebApi Long Running Request: {Name} ({ElapsedMilliseconds} milliseconds) {@UserId} {@UserName} {@Request}",
                requestName, elapsedMilliseconds, userId, userName, message);
        }
    }
}
