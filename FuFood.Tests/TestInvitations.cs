using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;
using FuFood.Data;
using FuFood.Models.Requests;
using FuFood.Repositories;
using Microsoft.Extensions.DependencyInjection;

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

        var aliceClient = ClientForUser(alice);
        var bobClient = ClientForUser(bob);

        // 用 Alice 的身份創建邀請碼
        var resp = await aliceClient.PostAsync($"/api/v1/refrigerators/{fr.Id}/invitations", null);
        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
        var json = await resp.Content.ReadAsStringAsync();
        var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;
        var invitationId = root.GetProperty("data").GetProperty("id").GetGuid();
        var token = root.GetProperty("data").GetProperty("token").GetString();
        Assert.NotNull(token);
        Assert.NotEmpty(token);
        doc.Dispose();

        var refrigeratorRepository = Fixture.AsyncScope.ServiceProvider.GetRequiredService<RefrigeratorRepository>();
        var actualFridge = await refrigeratorRepository.GetUserRefrigeratorById(bob, fr.Id);
        Assert.Null(actualFridge);

        // 用 Bob 的身份檢查邀請碼
        resp = await bobClient.GetAsync($"/api/v1/invitations/{token}");
        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
        json = await resp.Content.ReadAsStringAsync();
        doc = JsonDocument.Parse(json);
        root = doc.RootElement;

        // 驗證我們透過 token 拿到的邀請是否我們剛建立的同一筆資料
        var actualId = root.GetProperty("data").GetProperty("id").GetGuid();
        Assert.Equal(invitationId, actualId);
        doc.Dispose();

        var createRequest = new RefrigeratorMembershipCreateRequest
        {
            InvitationToken = token,
        };
        var jsonContent = new StringContent(
            JsonSerializer.Serialize(createRequest),
            System.Text.Encoding.UTF8,
            "application/json"
        );
        resp = await bobClient.PostAsync($"/api/v1/refrigerator_memberships", jsonContent);
        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);

        resp = await bobClient.GetAsync($"/api/v1/invitations/{token}");
        Assert.Equal(HttpStatusCode.NotFound, resp.StatusCode);

        var actual = await refrigeratorRepository.GetUserRefrigeratorById(bob, fr.Id);
        Assert.NotNull(actual);
    }
}