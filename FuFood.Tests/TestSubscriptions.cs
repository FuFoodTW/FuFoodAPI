using System.Net;
using FuFood.Models.Enums;

namespace FuFood.Tests;

[Collection("integration")]
public class TestSubscriptions(GlobalTestFixture fixture) : DbTestBase(fixture)
{
    [Fact]
    public async Task TestUpdateSubscriptionTier()
    {
        var user = await Factory.CreateUser();
        Assert.Equal(SubscriptionTier.Free, user.SubscriptionTier);

        var client = ClientForUser(user);
        var resp = await client.PostAsync("/api/v1/subscription", null);
        Assert.Equal(HttpStatusCode.NoContent, resp.StatusCode);
    }
}