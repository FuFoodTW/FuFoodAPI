using System.Net;
using FuFood.Data;
using FuFood.Models.Enums;
using Microsoft.Extensions.DependencyInjection;

namespace FuFood.Tests;

[Collection("integration")]
public class TestSubscriptions(GlobalTestFixture fixture) : DbTestBase(fixture)
{
    [Fact]
    public async Task TestUpdateSubscriptionTier()
    {
        var user = await Factory.CreateUser();
        Assert.Null(user.SubscriptionValidUntil);
        Assert.Equal(SubscriptionTier.Free, user.SubscriptionTier);

        var client = ClientForUser(user);
        var resp = await client.PostAsync("/api/v1/subscription", null);
        Assert.Equal(HttpStatusCode.NoContent, resp.StatusCode);

        var scope = Fixture.AsyncScope;
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // The POST request has side effects, therefore we need to reload the data from the database
        await dbContext.Entry(user).ReloadAsync();
        Assert.NotNull(user.SubscriptionValidUntil);
        Assert.Equal(SubscriptionTier.Pro, user.SubscriptionTier);

        var firstValidity = user.SubscriptionValidUntil;
        resp = await client.PostAsync("/api/v1/subscription", null);
        Assert.Equal(HttpStatusCode.NoContent, resp.StatusCode);
        await dbContext.Entry(user).ReloadAsync();
        Assert.NotNull(user.SubscriptionValidUntil);
        Assert.Equal(SubscriptionTier.Pro, user.SubscriptionTier);

        // Assert that the new time is longer than the first validity
        Assert.True(user.SubscriptionValidUntil > firstValidity);

        resp = await client.DeleteAsync("/api/v1/subscription");
        Assert.Equal(HttpStatusCode.NoContent, resp.StatusCode);

        await dbContext.Entry(user).ReloadAsync();
        Assert.Null(user.SubscriptionValidUntil);
        Assert.Equal(SubscriptionTier.Free, user.SubscriptionTier);
    }
}