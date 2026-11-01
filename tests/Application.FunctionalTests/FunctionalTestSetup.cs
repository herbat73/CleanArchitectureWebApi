using Microsoft.Extensions.DependencyInjection;

namespace CleanArchietureNoFrontEnd.Application.FunctionalTests;

// xUnit has no assembly-level setup fixture, so the shared host and database are created by a collection fixture.
// Every functional test class joins the "Functional" collection, which runs them sequentially against one database.
[CollectionDefinition(FunctionalTestSetup.CollectionName)]
public class FunctionalTestSetup : ICollectionFixture<FunctionalTestSetup>, IAsyncLifetime
{
    public const string CollectionName = "Functional";

    // Points at a dedicated database (not the development one) because the tests reset all tables between runs.
    // Override with the FUNCTIONAL_TESTS_CONNECTION_STRING environment variable.
    private const string DefaultConnectionString =
        "Server=127.0.0.1;Port=5432;Database=CleanArchietureNoFrontEndDb_FunctionalTests;Username=admin;Password=password;";

    internal static IServiceScopeFactory ScopeFactory { get; private set; } = null!;
    internal static DatabaseResetter? DbResetter { get; private set; }

    private static WebApiFactory? _factory;

    public async Task InitializeAsync()
    {
        var connectionString = Environment.GetEnvironmentVariable("FUNCTIONAL_TESTS_CONNECTION_STRING")
            ?? DefaultConnectionString;

        _factory = new WebApiFactory(connectionString);
        ScopeFactory = _factory.Services.GetRequiredService<IServiceScopeFactory>();
        DbResetter = await DatabaseResetter.CreateAsync(connectionString);
    }

    public async Task DisposeAsync()
    {
        if (DbResetter is not null) await DbResetter.DisposeAsync();
        if (_factory is not null) await _factory.DisposeAsync();
    }
}
