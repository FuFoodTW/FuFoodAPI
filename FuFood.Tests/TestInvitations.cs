using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace FuFood.Tests;

[Collection("integration")]
public class TestInvitations(GlobalTestFixture fixture) : DbTestBase(fixture)
{
    [Fact]
    public async Task TestAcceptInvitation()
    {
        var alice = await Factory.CreateUser();
        var bob = await Factory.CreateUser();

        var fr = await Factory.CreateRefrigerator(alice);

        var client = ClientForUser(alice);

        var resp = await client.PostAsync($"/api/v1/refrigerators/{fr.Id}/invitations", null);
        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
        var json = await resp.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;
        var invitationId = root.GetProperty("data").GetProperty("id").GetGuid();
    }
}