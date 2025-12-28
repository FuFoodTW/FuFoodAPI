using System.Net;

namespace FuFood.Tests;

[Collection("integration")]
public class TestProfile(GlobalTestFixture fixture) : DbTestBase(fixture)
{
    [Fact]
    public async Task TestUnauthenticated()
    {
        var client = Fixture.Factory.CreateDefaultClient();
        var response = await client.GetAsync("/api/v1/profile");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task TestWithBearerToken()
    {
        var user = await Factory.CreateUser();
        var client = Fixture.Factory.CreateDefaultClient();
        var token = TokenForUser(user);
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        var response = await client.GetAsync("/api/v1/profile");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}