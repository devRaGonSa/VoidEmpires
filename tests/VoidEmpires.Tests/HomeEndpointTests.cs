using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace VoidEmpires.Tests;

public class HomeEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public HomeEndpointTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task HomeEndpointIdentifiesApplication()
    {
        using var client = _factory.CreateClient();

        using var response = await client.GetAsync("/");
        var content = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("VoidEmpires", content);
    }
}
