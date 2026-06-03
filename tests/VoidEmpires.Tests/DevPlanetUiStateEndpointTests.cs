using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VoidEmpires.Application.Development;
using VoidEmpires.Application.Planets;
using VoidEmpires.Infrastructure.Development;
using VoidEmpires.Infrastructure.Persistence;
using VoidEmpires.Infrastructure.Planets;

namespace VoidEmpires.Tests;

public class DevPlanetUiStateEndpointTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private const string SeedCivilizationId = "00000000-0000-0000-0000-000000000001";
    private const string SeedOwnedPlanetId = "40000000-0000-0000-0000-000000000001";
    private const string SeedOuterPlanetId = "40000000-0000-0000-0000-000000000002";

    [Fact]
    public async Task PlanetUiStateReturnsNotFoundOutsideDevelopmentByDefault()
    {
        using var client = factory.WithWebHostBuilder(builder => builder.UseEnvironment("Production")).CreateClient();

        using var response = await client.GetAsync($"/api/dev/planets/ui-state?civilizationId={SeedCivilizationId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PlanetUiStateReturnsServiceUnavailableWhenPersistenceIsNotConfigured()
    {
        using var client = factory.CreateClientWithPersistenceDisabled();

        using var response = await client.GetAsync($"/api/dev/planets/ui-state?civilizationId={SeedCivilizationId}");

        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
    }

    [Fact]
    public async Task PlanetUiStateReturnsBadRequestWhenCivilizationIdIsMissing()
    {
        using var client = CreateConfiguredClient(CreateSeededDbContext());

        using var response = await client.GetAsync("/api/dev/planets/ui-state");
        var payload = await response.Content.ReadFromJsonAsync<DevPlanetUiStateResponse>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload.Succeeded);
        Assert.Contains("Civilization id is required.", payload.Errors);
    }

    [Fact]
    public async Task PlanetUiStateDefaultsToOwnedHomePlanetWhenPlanetIdIsNotProvided()
    {
        using var client = CreateConfiguredClient(CreateSeededDbContext());

        using var response = await client.GetAsync($"/api/dev/planets/ui-state?civilizationId={SeedCivilizationId}");
        var payload = await response.Content.ReadFromJsonAsync<DevPlanetUiStateResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload?.UiState?.Planet);
        Assert.True(payload.Succeeded);
        Assert.Equal(Guid.Parse(SeedOwnedPlanetId), payload.UiState.SelectedPlanetId);
        Assert.Equal("Aurelia", payload.UiState.Planet.PlanetName);
        Assert.True(payload.UiState.Planet.IsOwnedByRequestingCivilization);
        Assert.NotEmpty(payload.UiState.Planet.Stockpile);
        Assert.NotNull(payload.UiState.Planet.ProductionSummary);
        Assert.Contains(payload.UiState.Planet.Buildings, x => x.BuildingType.ToString() == "CommandCenter");
        Assert.Contains(payload.UiState.Planet.ConstructionActions, x => x.AvailabilityStatus == "Available");
        Assert.Contains(payload.UiState.Planet.ConstructionActions, x => x.AvailabilityStatus == "InsufficientResources");
        Assert.Equal("Available", payload.UiState.Planet.ActionSummary.QueueActionStatus);
        Assert.Contains(payload.UiState.Planet.Buildings, x => x.Display?.BuildingTypeLabel == "Centro de mando");
        Assert.Contains(payload.UiState.Planet.ConstructionActions, x => x.Display?.ActionLabel is "Construir" or "Mejorar");
        Assert.Equal("Disponible", payload.UiState.Planet.ActionSummary.Display?.QueueActionStatusLabel);
        Assert.False(payload.UiState.Planet.ActionSummary.CompleteDueSupported);
    }

    [Fact]
    public async Task PlanetUiStateAllowsExplicitForeignPlanetSelectionWithManagementDataHidden()
    {
        using var client = CreateConfiguredClient(CreateSeededDbContext());

        using var response = await client.GetAsync($"/api/dev/planets/ui-state?civilizationId={SeedCivilizationId}&planetId={SeedOuterPlanetId}");
        var payload = await response.Content.ReadFromJsonAsync<DevPlanetUiStateResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload?.UiState?.Planet);
        Assert.Equal(Guid.Parse(SeedOuterPlanetId), payload.UiState.SelectedPlanetId);
        Assert.False(payload.UiState.Planet.IsOwnedByRequestingCivilization);
        Assert.Empty(payload.UiState.Planet.Stockpile);
        Assert.Empty(payload.UiState.Planet.Buildings);
        Assert.Equal("Blocked", payload.UiState.Planet.ActionSummary.QueueActionStatus);
        Assert.Contains(payload.UiState.Planet.Diagnostics.Notes, x => x.Contains("non-owned", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task PlanetUiStateReturnsNotFoundForUnknownPlanet()
    {
        using var client = CreateConfiguredClient(CreateSeededDbContext());

        using var response = await client.GetAsync($"/api/dev/planets/ui-state?civilizationId={SeedCivilizationId}&planetId={Guid.NewGuid()}");
        var payload = await response.Content.ReadFromJsonAsync<DevPlanetUiStateResponse>();

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload.Succeeded);
        Assert.Contains("Planet was not found.", payload.Errors);
    }

    [Fact]
    public async Task PlanetFullValidationProfileReturnsRicherBuildingsAndCompletedQueueHistory()
    {
        using var client = CreateConfiguredClient(CreateSeededDbContext("planet-full-validation"));

        using var response = await client.GetAsync($"/api/dev/planets/ui-state?civilizationId={SeedCivilizationId}&planetId={SeedOwnedPlanetId}");
        var payload = await response.Content.ReadFromJsonAsync<DevPlanetUiStateResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload?.UiState?.Planet);
        Assert.True(payload.UiState.Planet.Buildings.Count >= 5);
        Assert.Contains(payload.UiState.Planet.Buildings, x => x.BuildingType.ToString() == "SolarPlant");
        Assert.Contains(payload.UiState.Planet.Buildings, x => x.BuildingType.ToString() == "MetalMine");
        Assert.Single(payload.UiState.Planet.ConstructionQueue);
        Assert.Equal("Completed", payload.UiState.Planet.ConstructionQueue[0].Status.ToString());
        Assert.True(payload.UiState.Planet.ConstructionActions.Count(x => x.AvailabilityStatus == "Available") >= 1);
        Assert.True(payload.UiState.Planet.ConstructionActions.Count(x => x.AvailabilityStatus != "Available") >= 3);
        Assert.Equal("Available", payload.UiState.Planet.ActionSummary.QueueActionStatus);
    }

    [Fact]
    public async Task ReapplyingPlanetFullValidationDoesNotDuplicateBuildingsOrCompletedQueueHistory()
    {
        await using var dbContext = CreateSeededDbContext("planet-full-validation");
        var service = new DevelopmentSeedService(dbContext);

        _ = await service.ApplyAsync(new ApplyDevelopmentSeedRequest("planet-full-validation"));

        var ownedPlanetId = Guid.Parse(SeedOwnedPlanetId);
        Assert.Equal(1, await dbContext.Set<VoidEmpires.Domain.Buildings.PlanetBuilding>().CountAsync(x => x.PlanetId == ownedPlanetId && x.BuildingType == VoidEmpires.Domain.Buildings.BuildingType.SolarPlant));
        Assert.Equal(1, await dbContext.Set<VoidEmpires.Domain.Buildings.PlanetBuilding>().CountAsync(x => x.PlanetId == ownedPlanetId && x.BuildingType == VoidEmpires.Domain.Buildings.BuildingType.MetalMine));
        Assert.Equal(1, await dbContext.Set<VoidEmpires.Domain.Buildings.PlanetConstructionOrder>().CountAsync(x =>
            x.PlanetId == ownedPlanetId &&
            x.BuildingType == VoidEmpires.Domain.Buildings.BuildingType.SolarPlant &&
            x.TargetLevel == 2 &&
            x.Status == VoidEmpires.Domain.Buildings.ConstructionQueueItemStatus.Completed));
    }

    private HttpClient CreateConfiguredClient(VoidEmpiresDbContext dbContext) =>
        factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Development");
            builder.ConfigureAppConfiguration((_, configurationBuilder) =>
                configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] = "Host=localhost;Database=voidempires_planet_ui_state_tests"
                }));
            builder.ConfigureTestServices(services =>
            {
                services.AddSingleton<IDevPlanetUiStateService>(new DevPlanetUiStateService(dbContext));
            });
        }).CreateClient();

    private static VoidEmpiresDbContext CreateSeededDbContext(string profile = "minimal-validation")
    {
        var dbContext = new VoidEmpiresDbContext(new DbContextOptionsBuilder<VoidEmpiresDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);
        new DevelopmentSeedService(dbContext).ApplyAsync(new ApplyDevelopmentSeedRequest(profile)).GetAwaiter().GetResult();
        return dbContext;
    }

    private sealed record DevPlanetUiStateResponse(
        bool Succeeded,
        GetDevPlanetUiStateResult? UiState,
        string[] Errors);
}
