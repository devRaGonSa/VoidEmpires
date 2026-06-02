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

public class DevPlanetVisualStateEndpointTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private static readonly Guid PlanetId = Guid.Parse("f8e7e0cc-957d-48d8-a9db-ef21fc5d1d86");
    private static readonly Guid CivilizationId = Guid.Parse("4953f3cc-d2c7-4f59-aa6c-639b913bf1f9");

    [Fact]
    public async Task PlanetVisualStateReturnsNotFoundOutsideDevelopmentByDefault()
    {
        using var client = factory.WithWebHostBuilder(builder => builder.UseEnvironment("Production")).CreateClient();

        using var response = await client.GetAsync($"/api/dev/planets/{PlanetId}/visual-state");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PlanetVisualStateReturnsServiceUnavailableWhenPersistenceIsNotConfigured()
    {
        using var client = CreateClientWithPersistenceDisabled("Development");

        using var response = await client.GetAsync($"/api/dev/planets/{PlanetId}/visual-state");

        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
    }

    [Fact]
    public async Task PlanetVisualStateReturnsServiceUnavailableOutsideDevelopmentWhenConfigOptInIsEnabled()
    {
        using var client = CreateClientWithPersistenceDisabled(
            "Production",
            devEndpointsEnabled: true);

        using var response = await client.GetAsync($"/api/dev/planets/{PlanetId}/visual-state");

        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
    }

    [Fact]
    public async Task PlanetVisualStateReturnsOkForSuccessfulState()
    {
        using var client = CreateConfiguredClient(new FakePlanetVisualStateService(GetPlanetVisualStateResult.Success(CreateVisualState())));

        using var response = await client.GetAsync($"/api/dev/planets/{PlanetId}/visual-state");
        var payload = await response.Content.ReadFromJsonAsync<PlanetVisualStateResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload);
        Assert.True(payload.Succeeded);
        Assert.Empty(payload.Errors);
        Assert.NotNull(payload.VisualState);
        Assert.Equal(PlanetId, payload.VisualState.PlanetId);
        Assert.Equal("Asterion", payload.VisualState.PlanetName);
        Assert.Equal(PlanetType.Terran, payload.VisualState.PlanetType);
        Assert.Equal(100, payload.VisualState.Size);
        Assert.True(payload.VisualState.IsOwned);
        Assert.Equal(CivilizationId, payload.VisualState.CivilizationId);
        Assert.Equal("hsl(120, 70%, 55%)", payload.VisualState.CivilizationColor);
        Assert.Equal("terran_continental", payload.VisualState.Profile.SurfaceProfile);
    }

    [Fact]
    public async Task PlanetVisualStateReturnsNotFoundWhenServiceCannotFindPlanet()
    {
        using var client = CreateConfiguredClient(new FakePlanetVisualStateService(GetPlanetVisualStateResult.Failure("Planet was not found.")));

        using var response = await client.GetAsync($"/api/dev/planets/{PlanetId}/visual-state");
        var payload = await response.Content.ReadFromJsonAsync<PlanetVisualStateResponse>();

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload.Succeeded);
        Assert.Null(payload.VisualState);
        Assert.Contains("Planet was not found.", payload.Errors);
    }

    private HttpClient CreateConfiguredClient(IPlanetVisualStateService visualStateService) =>
        factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Development");
            builder.ConfigureAppConfiguration((_, configurationBuilder) =>
                configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] = "Host=localhost;Database=voidempires_visual_endpoint_tests"
                }));
            builder.ConfigureTestServices(services => services.AddSingleton(visualStateService));
        }).CreateClient();

    private HttpClient CreateClientWithPersistenceDisabled(
        string environment,
        bool devEndpointsEnabled = false) =>
        factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment(environment);
            builder.ConfigureAppConfiguration((_, configurationBuilder) =>
                configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] = string.Empty,
                    ["VoidEmpires:DevEndpoints:Enabled"] = devEndpointsEnabled.ToString()
                }));
        }).CreateClient();

    private static PlanetVisualStateDto CreateVisualState() =>
        new(
            PlanetId,
            "Asterion",
            PlanetType.Terran,
            100,
            PlanetColonizationStatus.Colonized,
            IsOwned: true,
            CivilizationId,
            "hsl(120, 70%, 55%)",
            VisualSeed: 12345,
            ColonizationIntensity: 0.30f,
            UrbanIntensity: 0.05f,
            IndustrialIntensity: 0.25f,
            TerraformingIntensity: 0f,
            MilitaryIntensity: 0.10f,
            OrbitalPresenceIntensity: 0.50f,
            PlanetVisualProfileCatalog.GetProfile(PlanetType.Terran));

    private sealed class FakePlanetVisualStateService(GetPlanetVisualStateResult result) : IPlanetVisualStateService
    {
        public Task<GetPlanetVisualStateResult> GetAsync(
            GetPlanetVisualStateRequest request,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(result);
    }

    private sealed record PlanetVisualStateResponse(
        bool Succeeded,
        PlanetVisualStateDto? VisualState,
        string[] Errors);
}
