using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace VoidEmpires.Tests;

public class DevelopmentCorsEndpointTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private const string AllowedOrigin = "http://localhost:5173";
    private const string AlternateAllowedOrigin = "http://127.0.0.1:5173";
    private const string DisallowedOrigin = "http://localhost:4173";
    private static readonly Guid CivilizationId = Guid.Parse("00000000-0000-0000-0000-000000000001");

    [Theory]
    [InlineData(AllowedOrigin)]
    [InlineData(AlternateAllowedOrigin)]
    public async Task DevEndpointGetReturnsCorsHeadersForAllowedOrigins(string origin)
    {
        using var client = CreateDevelopmentClient();
        using var request = new HttpRequestMessage(HttpMethod.Get, $"/api/dev/fleets/ui-state?civilizationId={CivilizationId}");
        request.Headers.Add("Origin", origin);

        using var response = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
        Assert.Equal(origin, response.Headers.Single(x => x.Key == "Access-Control-Allow-Origin").Value.Single());
        Assert.Contains("Origin", response.Headers.Single(x => x.Key == "Vary").Value);
    }

    [Fact]
    public async Task DevEndpointOptionsReturnsCorsHeadersForAllowedPreflight()
    {
        using var client = CreateDevelopmentClient();
        using var request = new HttpRequestMessage(HttpMethod.Options, "/api/dev/fleets/ui-state");
        request.Headers.Add("Origin", AllowedOrigin);
        request.Headers.Add("Access-Control-Request-Method", "GET");
        request.Headers.Add("Access-Control-Request-Headers", "content-type");

        using var response = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Equal(AllowedOrigin, response.Headers.Single(x => x.Key == "Access-Control-Allow-Origin").Value.Single());
        Assert.Contains("GET", response.Headers.Single(x => x.Key == "Access-Control-Allow-Methods").Value.Single());
        Assert.Contains("content-type", response.Headers.Single(x => x.Key == "Access-Control-Allow-Headers").Value.Single(), StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task DevEndpointDoesNotReturnCorsHeadersForDisallowedOrigin()
    {
        using var client = CreateDevelopmentClient();
        using var request = new HttpRequestMessage(HttpMethod.Get, $"/api/dev/fleets/ui-state?civilizationId={CivilizationId}");
        request.Headers.Add("Origin", DisallowedOrigin);

        using var response = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
        Assert.False(response.Headers.Contains("Access-Control-Allow-Origin"));
    }

    [Fact]
    public async Task HealthEndpointDoesNotReturnCorsHeadersOutsideDevelopmentByDefault()
    {
        using var client = factory.WithWebHostBuilder(builder => builder.UseEnvironment("Production")).CreateClientWithPersistenceDisabled();
        using var request = new HttpRequestMessage(HttpMethod.Get, "/health");
        request.Headers.Add("Origin", AllowedOrigin);

        using var response = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.False(response.Headers.Contains("Access-Control-Allow-Origin"));
    }

    private HttpClient CreateDevelopmentClient() =>
        factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Development");
            builder.ConfigureAppConfiguration((_, configurationBuilder) =>
                configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] = string.Empty
                }));
        }).CreateClient();
}
