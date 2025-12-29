using FuFood.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FuFood.Tests;

public class GlobalTestFixture : IAsyncLifetime
{
    private WebApplicationFactory<Program>? _factory;

    public WebApplicationFactory<Program> Factory =>
        _factory ?? throw new InvalidOperationException("Factory is not initialized");

    public AsyncServiceScope AsyncScope => Factory.Services.CreateAsyncScope();

    public async Task InitializeAsync()
    {
        _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder => builder.UseEnvironment("Test"));
        await PrepareDb();
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    private async Task PrepareDb()
    {
        await using var scope = AsyncScope;
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync();
        await db.Users.ExecuteDeleteAsync();
    }
}