using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;

namespace CleanArchitectureWebApi.Application.FunctionalTests;

// xUnit has no assembly-level setup fixture, so the shared container, host and database are created by a collection fixture.
// Every functional test class joins the "Functional" collection, which runs them sequentially against one database.
[CollectionDefinition(FunctionalTestSetup.CollectionName)]
public class FunctionalTestSetup : ICollectionFixture<FunctionalTestSetup>, IAsyncLifetime
{
    public const string CollectionName = "Functional";

    // Matches the image used by docker-compose.yml so the tests run against the same PostgreSQL version as development.
    private const string PostgresImage = "postgres:18.3";

    // A dedicated database, not the container's default "postgres" one: the development initialiser drops and recreates
    // its target database on startup, which PostgreSQL refuses for the maintenance database.
    private const string DatabaseName = "CleanArchietureNoFrontEndDb_FunctionalTests";

    private PostgreSqlContainer? _container;
    private WebApiFactory? _factory;

    internal static IServiceScopeFactory ScopeFactory { get; private set; } = null!;
    internal static DatabaseResetter? DbResetter { get; private set; }

    public async Task InitializeAsync()
    {
        // A throwaway PostgreSQL container per test run: the tests reset all tables, so they never touch a development database.
        _container = new PostgreSqlBuilder(PostgresImage)
            .WithDatabase(DatabaseName)
            .Build();
        await _container.StartAsync();

        var connectionString = _container.GetConnectionString();

        _factory = new WebApiFactory(connectionString);
        ScopeFactory = _factory.Services.GetRequiredService<IServiceScopeFactory>();
        DbResetter = await DatabaseResetter.CreateAsync(connectionString);
    }

    public async Task DisposeAsync()
    {
        if (DbResetter is not null) await DbResetter.DisposeAsync();
        if (_factory is not null) await _factory.DisposeAsync();
        if (_container is not null) await _container.DisposeAsync();
    }
}
