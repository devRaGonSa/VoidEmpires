using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VoidEmpires.Application.Visuals;
using VoidEmpires.Domain.Galaxy;

namespace VoidEmpires.Tests;

public class DevSystemVisualStateEndpointTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private static readonly Guid SystemId = Guid.Parse("4cfb3562-0e91-4cf0-98bd-44309f2ff98e");
    private static readonly Guid GalaxyId = Guid.Parse("ac6f0941-bbe4-4778-86fa-03eba119a5c0");
    private static readonly Guid StarId = Guid.Parse("cc31bf95-ee0c-49bd-a7f4-d3cc26f53455");
    private static readonly Guid PlanetId = Guid.Parse("aa6c3794-2fa5-4567-85a8-e71690657f98");

    [Fact]
    public async Task SystemVisualStateReturnsNotFoundOutsideDevelopmentByDefault()
    {
        using var client = factory.WithWebHostBuilder(builder => builder.UseEnvironment("Production")).CreateClient();

        using var response = await client.GetAsync($"/api/dev/solar-systems/{SystemId}/visual-state");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task SystemVisualStateReturnsServiceUnavailableWhenPersistenceIsNotConfigured()
    {
        using var client = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Development");
            builder.ConfigureAppConfiguration((_, configurationBuilder) =>
                configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] = string.Empty
                }));
        }).CreateClient();

        using var response = await client.GetAsync($"/api/dev/solar-systems/{SystemId}/visual-state");

        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
    }

    [Fact]
    public async Task SystemVisualStateReturnsOkForSuccessfulState()
    {
        using var client = CreateConfiguredClient(new FakeSystemVisualStateService(GetSystemVisualStateResult.Success(CreateVisualState())));

        using var response = await client.GetAsync($"/api/dev/solar-systems/{SystemId}/visual-state");
        var payload = await response.Content.ReadFromJsonAsync<SystemVisualStateResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload);
        Assert.True(payload.Succeeded);
        Assert.Empty(payload.Errors);
        Assert.NotNull(payload.VisualState);
        Assert.Equal(SystemId, payload.VisualState.SystemId);
        Assert.Equal(GalaxyId, payload.VisualState.GalaxyId);
        Assert.Equal("Helios Prime", payload.VisualState.SystemName);
        Assert.Equal(StarId, payload.VisualState.Star.StarId);
        Assert.Equal("yellow_dwarf", payload.VisualState.Star.VisualClass);
        var hint = Assert.Single(payload.VisualState.LayoutHints);
        Assert.Equal(PlanetId, hint.PlanetId);
        var planet = Assert.Single(payload.VisualState.Planets);
        Assert.Equal(PlanetId, planet.PlanetId);
        Assert.Equal("Asterion", planet.PlanetName);
    }

    [Fact]
    public async Task SystemVisualStateReturnsNotFoundWhenServiceCannotFindSystem()
    {
        using var client = CreateConfiguredClient(new FakeSystemVisualStateService(GetSystemVisualStateResult.Failure("System was not found.")));

        using var response = await client.GetAsync($"/api/dev/solar-systems/{SystemId}/visual-state");
        var payload = await response.Content.ReadFromJsonAsync<SystemVisualStateResponse>();

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload.Succeeded);
        Assert.Null(payload.VisualState);
        Assert.Contains("System was not found.", payload.Errors);
    }

    private HttpClient CreateConfiguredClient(ISystemVisualStateService visualStateService) =>
        factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Development");
            builder.ConfigureAppConfiguration((_, configurationBuilder) =>
                configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] = "Host=localhost;Database=voidempires_system_visual_endpoint_tests"
                }));
            builder.ConfigureTestServices(services => services.AddSingleton(visualStateService));
        }).CreateClient();

    private static SystemVisualStateDto CreateVisualState() =>
        new(
            SystemId,
            GalaxyId,
            "Helios Prime",
            CoordinateX: 1,
            CoordinateY: 2,
            CoordinateZ: 3,
            new StarVisualStateDto(StarId, "Helios", StarType.YellowDwarf, "yellow_dwarf", 0.75f),
            [new PlanetVisualLayoutHintDto(PlanetId, OrbitalSlot: 1, OrbitRadius: 5.75f, OrbitAngleDegrees: 47f, VisualScale: 1f)],
            [CreatePlanetState()]);

    private static PlanetVisualStateDto CreatePlanetState() =>
        new(
            PlanetId,
            "Asterion",
            PlanetType.Terran,
            100,
            PlanetColonizationStatus.Uncolonized,
            IsOwned: false,
            CivilizationId: null,
            CivilizationColor: null,
            VisualSeed: 123,
            ColonizationIntensity: 0f,
            UrbanIntensity: 0f,
            IndustrialIntensity: 0f,
            TerraformingIntensity: 0f,
            MilitaryIntensity: 0f,
            OrbitalPresenceIntensity: 0f,
            PlanetVisualProfileCatalog.GetProfile(PlanetType.Terran));

    private sealed class FakeSystemVisualStateService(GetSystemVisualStateResult result) : ISystemVisualStateService
    {
        public Task<GetSystemVisualStateResult> GetAsync(
            GetSystemVisualStateRequest request,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(result);
    }

    private sealed record SystemVisualStateResponse(
        bool Succeeded,
        SystemVisualStateDto? VisualState,
        string[] Errors);
}
