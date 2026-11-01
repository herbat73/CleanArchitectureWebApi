namespace CleanArchitectureWebApi.Application.FunctionalTests.Infrastructure;

// xUnit creates a new instance of the test class for every test, so InitializeAsync runs before each test
// (xUnit's equivalent of a per-test setup method).
public abstract class TestBase : IAsyncLifetime
{
    public async Task InitializeAsync()
    {
        await TestApp.ResetState();
    }

    public Task DisposeAsync() => Task.CompletedTask;
}
