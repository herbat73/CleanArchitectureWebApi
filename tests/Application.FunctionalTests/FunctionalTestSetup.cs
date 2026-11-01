using Microsoft.Extensions.DependencyInjection;

namespace CleanArchietureNoFrontEnd.Application.FunctionalTests;

[SetUpFixture]
public class FunctionalTestSetup
{
    // Points at a dedicated database (not the development one) because the tests reset all tables between runs.
    // Override with the FUNCTIONAL_TESTS_CONNECTION_STRING environment variable.
    private const string DefaultConnectionString =
        "Server=127.0.0.1;Port=5432;Database=CleanArchietureNoFrontEndDb_FunctionalTests;Username=admin;Password=password;";

    internal static IServiceScopeFactory ScopeFactory { get; private set; } = null!;
    internal static DatabaseResetter? DbResetter { get; private set; }

    private static WebApiFactory? _factory;

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        var connectionString = Environment.GetEnvironmentVariable("FUNCTIONAL_TESTS_CONNECTION_STRING")
            ?? DefaultConnectionString;

        _factory = new WebApiFactory(connectionString);
        ScopeFactory = _factory.Services.GetRequiredService<IServiceScopeFactory>();
        DbResetter = await DatabaseResetter.CreateAsync(connectionString);
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        if (DbResetter is not null) await DbResetter.DisposeAsync();
        if (_factory is not null) await _factory.DisposeAsync();
    }
}
