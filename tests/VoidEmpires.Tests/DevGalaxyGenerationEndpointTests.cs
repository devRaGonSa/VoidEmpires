using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VoidEmpires.Application.Galaxy;

namespace VoidEmpires.Tests;

public class DevGalaxyGenerationEndpointTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task GenerateGalaxyReturnsNotFoundOutsideDevelopmentByDefault()
    {
        using var client = factory.WithWebHostBuilder(builder => builder.UseEnvironment("Production")).CreateClient();

        using var response = await client.PostAsJsonAsync("/api/dev/galaxies/generate", ValidRequest());

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GenerateGalaxyReturnsServiceUnavailableWhenPersistenceIsNotConfigured()
    {
        using var client = factory.CreateClientWithPersistenceDisabled();

        using var response = await client.PostAsJsonAsync("/api/dev/galaxies/generate", ValidRequest());

        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
    }

    [Fact]
    public async Task GenerateGalaxyReturnsBadRequestForInvalidRequest()
    {
        using var client = CreateConfiguredClient(GenerateAndPersistGalaxyResult.Success(Guid.NewGuid(), "Unused", 1, 1));

        using var response = await client.PostAsJsonAsync("/api/dev/galaxies/generate", new { name = "", seed = "" });
        var payload = await response.Content.ReadFromJsonAsync<GalaxyGenerationResponse>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload.Succeeded);
        Assert.Contains("Galaxy name is required.", payload.Errors);
        Assert.Contains("Galaxy seed is required.", payload.Errors);
    }

    [Fact]
    public async Task GenerateGalaxyReturnsCreatedForSuccessfulGeneration()
    {
        var galaxyId = Guid.Parse("46c834e8-5190-418c-9f6f-589ff3889c98");
        using var client = CreateConfiguredClient(GenerateAndPersistGalaxyResult.Success(galaxyId, "Void Prime", 12, 42));

        using var response = await client.PostAsJsonAsync("/api/dev/galaxies/generate", ValidRequest());
        var payload = await response.Content.ReadFromJsonAsync<GalaxyGenerationResponse>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(payload);
        Assert.True(payload.Succeeded);
        Assert.Equal(galaxyId, payload.GalaxyId);
        Assert.Equal("Void Prime", payload.GalaxyName);
        Assert.Equal(12, payload.SolarSystemCount);
        Assert.Equal(42, payload.PlanetCount);
        Assert.Empty(payload.Errors);
    }

    private HttpClient CreateConfiguredClient(GenerateAndPersistGalaxyResult result) =>
        factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((_, configurationBuilder) =>
                configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] = "Host=localhost;Database=voidempires_dev_endpoint_tests"
                }));
            builder.ConfigureTestServices(services =>
                services.AddSingleton<IGalaxyGenerationService>(new FakeGalaxyGenerationService(result)));
        }).CreateClient();

    private static object ValidRequest() => new
    {
        name = "Void Prime",
        seed = "alpha-001",
        solarSystemCount = 12,
        minPlanetsPerSystem = 2,
        maxPlanetsPerSystem = 5
    };

    private sealed class FakeGalaxyGenerationService(GenerateAndPersistGalaxyResult result) : IGalaxyGenerationService
    {
        public Task<GenerateAndPersistGalaxyResult> GenerateAndPersistAsync(
            GenerateAndPersistGalaxyRequest request,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(result);
    }

    private sealed record GalaxyGenerationResponse(
        bool Succeeded,
        Guid? GalaxyId,
        string? GalaxyName,
        int SolarSystemCount,
        int PlanetCount,
        string[] Errors);
}
