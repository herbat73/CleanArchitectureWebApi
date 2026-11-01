using CleanArchietureNoFrontEnd.Application.Common.Interfaces;

namespace CleanArchietureNoFrontEnd.Application.FunctionalTests.Infrastructure;

/// <summary>
/// Concrete IUser for the test host. Wolverine generates typed handler/middleware code and cannot
/// resolve opaque lambda registrations, so this reads the current test identity from TestApp.
/// </summary>
public class TestUser : IUser
{
    public string? Id => TestApp.GetUserId();

    public List<string>? Roles => TestApp.GetRoles();
}
