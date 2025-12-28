using System.Net;

namespace FuFood.Tests;

[Collection("integration")]
public class UnitTest1(GlobalTestFixture fixture) : DbTestBase(fixture)
{
    [Fact]
    public void Test1()
    {
        var actual = 2 + 2;
        Assert.Equal(4, actual);
    }

    [Fact]
    public async Task Test_GetRoot()
    {
        var client = Fixture.Factory.CreateDefaultClient();
        var response = await client.GetAsync("/");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}