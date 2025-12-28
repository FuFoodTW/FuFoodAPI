namespace FuFood.Tests;

public abstract class DbTestBase(GlobalTestFixture fixture) : IAsyncLifetime
{
    protected readonly GlobalTestFixture Fixture = fixture;

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}