using System.Reflection;
using CleanArchitectureWebApi.Application.Common.Exceptions;
using CleanArchitectureWebApi.Application.Common.Interfaces;
using CleanArchitectureWebApi.Application.Common.Security;
using Wolverine;

namespace CleanArchitectureWebApi.Application.Common.Middleware;

public class AuthorizationMiddleware
{
    public static async Task Before(Envelope envelope, IUser user, IIdentityService identityService)
    {
        var message = envelope.Message!;
        var authorizeAttributes = message.GetType().GetCustomAttributes<AuthorizeAttribute>();

        if (!authorizeAttributes.Any())
        {
            // Authorization not required
            return;
        }

        // Must be authenticated user
        if (user.Id == null)
        {
            throw new UnauthorizedAccessException();
        }

        // Role-based authorization
        var authorizeAttributesWithRoles = authorizeAttributes.Where(a => !string.IsNullOrWhiteSpace(a.Roles));

        if (authorizeAttributesWithRoles.Any())
        {
            var authorized = false;

            foreach (var roles in authorizeAttributesWithRoles.Select(a => a.Roles.Split(',')))
            {
                foreach (var role in roles)
                {
                    var isInRole = user.Roles?.Any(x => role == x) ?? false;
                    if (isInRole)
                    {
                        authorized = true;
                        break;
                    }
                }
            }

            // Must be a member of at least one role in roles
            if (!authorized)
            {
                throw new ForbiddenAccessException();
            }
        }

        // Policy-based authorization
        var authorizeAttributesWithPolicies = authorizeAttributes.Where(a => !string.IsNullOrWhiteSpace(a.Policy));

        foreach (var policy in authorizeAttributesWithPolicies.Select(a => a.Policy))
        {
            var authorized = await identityService.AuthorizeAsync(user.Id, policy);

            if (!authorized)
            {
                throw new ForbiddenAccessException();
            }
        }
    }
}
