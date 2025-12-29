using System.Security.Cryptography;
using FuFood.Data;
using FuFood.Models.Entities;
using FuFood.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FuFood.Tests;

public abstract class DbTestBase(GlobalTestFixture fixture) : IAsyncLifetime
{
    protected readonly GlobalTestFixture Fixture = fixture;

    private AsyncServiceScope? _scope;

    protected AsyncServiceScope Scope => _scope ?? throw new InvalidOperationException("Scope is not initialized");
    protected AppDbContext DbContext => Scope.ServiceProvider.GetRequiredService<AppDbContext>();
    protected TestFactory Factory => new TestFactory(DbContext);

    public Task InitializeAsync()
    {
        _scope = Fixture.AsyncScope;
        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        await Scope.DisposeAsync();
    }

    protected string TokenForUser(User user)
    {
        using var scope = Fixture.AsyncScope;
        var jwtService = scope.ServiceProvider.GetRequiredService<JwtService>();
        return jwtService.IssueAccessTokenForUser(user);
    }

    protected HttpClient ClientForUser(User user)
    {
        var token = TokenForUser(user);
        var client = Fixture.Factory.CreateDefaultClient();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        return client;
    }
}