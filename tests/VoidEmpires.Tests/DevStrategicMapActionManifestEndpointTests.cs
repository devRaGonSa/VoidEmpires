using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using VoidEmpires.Application.StrategicMap;

namespace VoidEmpires.Tests;

public class DevStrategicMapActionManifestEndpointTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task ActionManifestReturnsNotFoundOutsideDevelopmentByDefault()
    {
        using var client = factory.WithWebHostBuilder(builder => builder.UseEnvironment("Production")).CreateClient();

        using var response = await client.GetAsync("/api/dev/strategic-map/action-manifest");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task ActionManifestReturnsDeterministicManifest()
    {
        using var client = factory.WithWebHostBuilder(builder => builder.UseEnvironment("Development")).CreateClient();

        using var firstResponse = await client.GetAsync("/api/dev/strategic-map/action-manifest");
        using var secondResponse = await client.GetAsync("/api/dev/strategic-map/action-manifest");
        var firstContent = await firstResponse.Content.ReadAsStringAsync();
        var secondContent = await secondResponse.Content.ReadAsStringAsync();
        var payload = await firstResponse.Content.ReadFromJsonAsync<StrategicMapActionManifestResponse>();

        Assert.Equal(HttpStatusCode.OK, firstResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, secondResponse.StatusCode);
        Assert.Equal(firstContent, secondContent);
        Assert.NotNull(payload);
        Assert.True(payload.Succeeded);
        Assert.Empty(payload.Errors);
        Assert.NotNull(payload.Manifest);
        Assert.Contains(payload.Manifest.Actions, x => x.ActionKey == "strategicMap.read");
        Assert.Contains(payload.Manifest.Actions, x => x.ActionKey == "strategicMap.actionManifest.read");
    }

    private sealed record StrategicMapActionManifestResponse(
        bool Succeeded,
        GetDevStrategicMapActionManifestResult? Manifest,
        string[] Errors);
}
