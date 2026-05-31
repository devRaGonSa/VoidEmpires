using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace VoidEmpires.Tests;

public class VisualStateSandboxEndpointTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task VisualStateSandboxHtmlIsServedInDevelopment()
    {
        using var client = factory.WithWebHostBuilder(builder => builder.UseEnvironment("Development")).CreateClient();

        using var response = await client.GetAsync("/dev/visual-state/index.html");
        var content = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("Visual State Inspector", content);
        Assert.Contains("Render mode", content);
        Assert.Contains("selected-planet-label", content);
        Assert.Contains("visual-profile", content);
        Assert.Contains("visual-overlays", content);
        Assert.Contains("Overlays", content);
        Assert.Contains("/dev/visual-state/visual-state.css", content);
        Assert.Contains("/dev/visual-state/visual-state.js", content);
    }

    [Fact]
    public async Task VisualStateSandboxHtmlIsNotServedOutsideDevelopmentByDefault()
    {
        using var client = factory.WithWebHostBuilder(builder => builder.UseEnvironment("Production")).CreateClient();

        using var response = await client.GetAsync("/dev/visual-state/index.html");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task VisualStateSandboxHtmlIsServedOutsideDevelopmentWhenDevEndpointsAreEnabled()
    {
        using var client = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Production");
            builder.ConfigureAppConfiguration((_, configurationBuilder) =>
                configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["VoidEmpires:DevEndpoints:Enabled"] = "true"
                }));
        }).CreateClient();

        using var response = await client.GetAsync("/dev/visual-state/index.html");
        var content = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("Visual State Inspector", content);
    }

    [Fact]
    public async Task VisualStateSandboxCssIsServedInDevelopment()
    {
        using var client = factory.WithWebHostBuilder(builder => builder.UseEnvironment("Development")).CreateClient();

        using var response = await client.GetAsync("/dev/visual-state/visual-state.css");
        var content = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("sandbox-shell", content);
        Assert.Contains("pseudo-scene", content);
        Assert.Contains("pseudo-planet", content);
        Assert.Contains("system-orbits", content);
        Assert.Contains("profile-grid", content);
        Assert.Contains("overlay-list", content);
        Assert.Contains("system-group-marker", content);
        Assert.Contains("system-transfer-overlay", content);
        Assert.Contains("system-transfer-progress", content);
    }

    [Fact]
    public async Task VisualStateSandboxScriptIsServedInDevelopment()
    {
        using var client = factory.WithWebHostBuilder(builder => builder.UseEnvironment("Development")).CreateClient();

        using var response = await client.GetAsync("/dev/visual-state/visual-state.js");
        var content = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("loadVisualState", content);
        Assert.Contains("renderSystem", content);
        Assert.Contains("renderPlanet", content);
        Assert.Contains("renderProfile", content);
        Assert.Contains("renderOverlays", content);
        Assert.Contains("addOrbitalGroupMarker", content);
        Assert.Contains("addTransferOverlay", content);
        Assert.Contains("orbitalGroupMarkers", content);
        Assert.Contains("transferOverlays", content);
        Assert.Contains("/api/dev/planets/", content);
        Assert.Contains("/api/dev/solar-systems/", content);
    }
}
