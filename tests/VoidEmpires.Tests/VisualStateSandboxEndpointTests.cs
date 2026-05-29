using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace VoidEmpires.Tests;

public class VisualStateSandboxEndpointTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task VisualStateSandboxHtmlIsServed()
    {
        using var client = factory.CreateClient();

        using var response = await client.GetAsync("/dev/visual-state/index.html");
        var content = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("Visual State Inspector", content);
        Assert.Contains("/dev/visual-state/visual-state.css", content);
        Assert.Contains("/dev/visual-state/visual-state.js", content);
    }

    [Fact]
    public async Task VisualStateSandboxCssIsServed()
    {
        using var client = factory.CreateClient();

        using var response = await client.GetAsync("/dev/visual-state/visual-state.css");
        var content = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("sandbox-shell", content);
    }

    [Fact]
    public async Task VisualStateSandboxScriptIsServed()
    {
        using var client = factory.CreateClient();

        using var response = await client.GetAsync("/dev/visual-state/visual-state.js");
        var content = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("loadVisualState", content);
        Assert.Contains("/api/dev/planets/", content);
        Assert.Contains("/api/dev/solar-systems/", content);
    }
}
