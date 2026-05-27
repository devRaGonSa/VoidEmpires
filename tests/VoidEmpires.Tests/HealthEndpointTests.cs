using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace VoidEmpires.Tests;

public class HealthEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public HealthEndpointTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task HealthEndpointReturnsExpectedPayload()
    {
        using var client = _factory.CreateClient();

        using var response = await client.GetAsync("/health");
        var payload = await response.Content.ReadFromJsonAsync<HealthResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload);
        Assert.Equal("ok", payload.Status);
        Assert.Equal("VoidEmpires.Web", payload.Service);
        Assert.False(payload.Persistence.Configured);
        Assert.Equal("PostgreSQL", payload.Persistence.Provider);
        Assert.False(payload.Auth.Configured);
        Assert.Equal("ASP.NET Core Identity", payload.Auth.Provider);
    }

    [Fact]
    public async Task HealthEndpointReportsConfiguredPersistenceWithoutExposingConnectionString()
    {
        var factory = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((_, configurationBuilder) =>
            {
                configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] = "Host=localhost;Database=voidempires_test"
                });
            });
        });
        using var client = factory.CreateClient();

        using var response = await client.GetAsync("/health");
        var content = await response.Content.ReadAsStringAsync();
        var payload = await response.Content.ReadFromJsonAsync<HealthResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload);
        Assert.True(payload.Persistence.Configured);
        Assert.Equal("PostgreSQL", payload.Persistence.Provider);
        Assert.True(payload.Auth.Configured);
        Assert.Equal("ASP.NET Core Identity", payload.Auth.Provider);
        Assert.DoesNotContain("localhost", content);
        Assert.DoesNotContain("voidempires_test", content);
    }

    private sealed record HealthResponse(string Status, string Service, PersistenceHealth Persistence, AuthHealth Auth);

    private sealed record PersistenceHealth(bool Configured, string Provider);

    private sealed record AuthHealth(bool Configured, string Provider);
}
