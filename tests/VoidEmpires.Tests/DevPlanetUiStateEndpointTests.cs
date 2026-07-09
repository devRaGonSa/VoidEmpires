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
using VoidEmpires.Domain.Buildings;
using VoidEmpires.Domain.Economy;
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
        Assert.Equal(7, payload.UiState.Planet.ResourceCatalog.Count);
        Assert.Equal("Credits", payload.UiState.Planet.ResourceCatalog[0].Key);
        Assert.Equal("Creditos", payload.UiState.Planet.ResourceCatalog[0].DisplayName);
        Assert.Equal("Energy", payload.UiState.Planet.ResourceCatalog[4].Key);
        Assert.False(payload.UiState.Planet.ResourceCatalog[4].IsPersisted);
        Assert.False(payload.UiState.Planet.ResourceCatalog[4].IsSpendable);
        Assert.NotEmpty(payload.UiState.Planet.Stockpile);
        Assert.All(payload.UiState.Planet.Stockpile, item =>
        {
            Assert.NotNull(item.Capacity);
            Assert.True(item.Capacity >= item.Quantity);
        });
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
    public async Task PlanetUiStateRepairsMissingBuildingCapacityForOwnedPlanet()
    {
        await using var dbContext = CreateSeededDbContext();
        var ownedPlanetId = Guid.Parse(SeedOwnedPlanetId);
        var staleCapacity = await dbContext.PlanetBuildingCapacities.SingleAsync(x => x.PlanetId == ownedPlanetId);
        dbContext.PlanetBuildingCapacities.Remove(staleCapacity);
        await dbContext.SaveChangesAsync();

        using var client = CreateConfiguredClient(dbContext);

        using var response = await client.GetAsync($"/api/dev/planets/ui-state?civilizationId={SeedCivilizationId}&planetId={SeedOwnedPlanetId}");
        var payload = await response.Content.ReadFromJsonAsync<DevPlanetUiStateResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload?.UiState?.Planet);
        Assert.NotNull(payload.UiState.Planet.BuildingCapacity);
        Assert.True(payload.UiState.Planet.Diagnostics.HasBuildingCapacity);
        Assert.DoesNotContain(
            payload.UiState.Planet.ConstructionActions,
            action => action.AvailabilityStatus == "MissingCapacityData");
        Assert.Contains(
            payload.UiState.Planet.ConstructionActions,
            action => action.BuildingType == BuildingType.MetalMine && action.AvailabilityStatus == "Available");
        Assert.True(await dbContext.PlanetBuildingCapacities.AnyAsync(x => x.PlanetId == ownedPlanetId));
    }

    [Fact]
    public async Task PlanetUiStateUsesHighestLevelWhenLegacyDuplicateBuildingsExist()
    {
        await using var dbContext = CreateSeededDbContext();
        var ownedPlanetId = Guid.Parse(SeedOwnedPlanetId);
        dbContext.PlanetBuildings.Add(PlanetBuilding.Create(ownedPlanetId, BuildingType.CommandCenter, 7, 12));
        await dbContext.SaveChangesAsync();
        using var client = CreateConfiguredClient(dbContext);

        using var response = await client.GetAsync($"/api/dev/planets/ui-state?civilizationId={SeedCivilizationId}&planetId={SeedOwnedPlanetId}");
        var payload = await response.Content.ReadFromJsonAsync<DevPlanetUiStateResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload?.UiState?.Planet);
        var commandCenter = Assert.Single(payload.UiState.Planet.Buildings, x => x.BuildingType == BuildingType.CommandCenter);
        var commandCenterAction = Assert.Single(payload.UiState.Planet.ConstructionActions, x => x.BuildingType == BuildingType.CommandCenter);
        Assert.Equal(7, commandCenter.Level);
        Assert.Equal(12, commandCenter.Footprint);
        Assert.Equal(7, commandCenterAction.CurrentLevel);
        Assert.Equal(8, commandCenterAction.TargetLevel);
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
    public async Task CockpitValidationProfileReturnsOwnedPlanetResourcesAndCompletedConstructionHistory()
    {
        using var client = CreateConfiguredClient(CreateSeededDbContext("cockpit-validation"));

        using var response = await client.GetAsync($"/api/dev/planets/ui-state?civilizationId={SeedCivilizationId}&planetId={SeedOwnedPlanetId}");
        var payload = await response.Content.ReadFromJsonAsync<DevPlanetUiStateResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload?.UiState?.Planet);
        Assert.True(payload.UiState.Planet.IsOwnedByRequestingCivilization);
        Assert.NotEmpty(payload.UiState.Planet.Stockpile);
        Assert.Single(payload.UiState.Planet.ConstructionQueue);
        Assert.Equal("Completed", payload.UiState.Planet.ConstructionQueue[0].Status.ToString());
        Assert.True(payload.UiState.Planet.ConstructionActions.Count(x => x.AvailabilityStatus == "Available") >= 1);
        Assert.True(payload.UiState.Planet.ConstructionActions.Count(x => x.AvailabilityStatus != "Available") >= 1);
    }

    [Fact]
    public async Task ApplyPlanetResourceEconomyPersistsAccrualAndUiStateRemainsReadOnly()
    {
        var databaseName = Guid.NewGuid().ToString("N");
        using var configuredFactory = factory.WithInMemoryPersistence(databaseName: databaseName);
        using var client = configuredFactory.CreateClient();

        using (var seedResponse = await client.PostAsJsonAsync("/api/dev/seeds/apply", new { profile = "minimal-validation" }))
        {
            Assert.Equal(HttpStatusCode.OK, seedResponse.StatusCode);
        }

        decimal firstCreditsBefore, firstMetalBefore, firstCrystalBefore, firstGasBefore;

        using (var scope = configuredFactory.Services.CreateScope())
        {
            var setupDbContext = scope.ServiceProvider.GetRequiredService<VoidEmpiresDbContext>();
            setupDbContext.PlanetResourceStockpiles.Add(PlanetResourceStockpile.Create(Guid.Parse(SeedOuterPlanetId)));
            var baselineStockpile = await setupDbContext.PlanetResourceStockpiles.AsNoTracking().SingleAsync(x => x.PlanetId == Guid.Parse(SeedOwnedPlanetId));
            (firstCreditsBefore, firstMetalBefore, firstCrystalBefore, firstGasBefore) = (
                baselineStockpile.Credits,
                baselineStockpile.Metal,
                baselineStockpile.Crystal,
                baselineStockpile.Gas);
            await setupDbContext.SaveChangesAsync();
        }

        using var applyResponse = await client.PostAsJsonAsync("/api/dev/planets/resource-economy/apply", new
        {
            civilizationId = SeedCivilizationId,
            planetId = SeedOwnedPlanetId,
            elapsedSeconds = 3600
        });
        Assert.Equal(HttpStatusCode.OK, applyResponse.StatusCode);

        using var firstUiResponse = await client.GetAsync($"/api/dev/planets/ui-state?civilizationId={SeedCivilizationId}&planetId={SeedOwnedPlanetId}");
        var firstUiPayload = await firstUiResponse.Content.ReadFromJsonAsync<DevPlanetUiStateResponse>();
        using var secondUiResponse = await client.GetAsync($"/api/dev/planets/ui-state?civilizationId={SeedCivilizationId}&planetId={SeedOwnedPlanetId}");
        var secondUiPayload = await secondUiResponse.Content.ReadFromJsonAsync<DevPlanetUiStateResponse>();

        Assert.Equal(HttpStatusCode.OK, firstUiResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, secondUiResponse.StatusCode);
        Assert.NotNull(firstUiPayload?.UiState?.Planet);
        Assert.NotNull(secondUiPayload?.UiState?.Planet);
        Assert.Equal(firstCreditsBefore + 18m, firstUiPayload.UiState.Planet.Stockpile.Single(x => x.ResourceType == ResourceType.Credits).Quantity);
        Assert.Equal(firstCreditsBefore + 18m, secondUiPayload.UiState.Planet.Stockpile.Single(x => x.ResourceType == ResourceType.Credits).Quantity);

        using var verificationScope = configuredFactory.Services.CreateScope();
        var verificationDbContext = verificationScope.ServiceProvider.GetRequiredService<VoidEmpiresDbContext>();
        var ownedStockpile = await verificationDbContext.PlanetResourceStockpiles.AsNoTracking().SingleAsync(x => x.PlanetId == Guid.Parse(SeedOwnedPlanetId));
        var foreignStockpile = await verificationDbContext.PlanetResourceStockpiles.AsNoTracking().SingleAsync(x => x.PlanetId == Guid.Parse(SeedOuterPlanetId));
        Assert.Equal(firstCreditsBefore + 18m, ownedStockpile.Credits);
        Assert.Equal(firstMetalBefore + 14m, ownedStockpile.Metal);
        Assert.Equal(firstCrystalBefore + 6m, ownedStockpile.Crystal);
        Assert.Equal(firstGasBefore + 3m, ownedStockpile.Gas);
        Assert.Equal(0m, foreignStockpile.Credits);
        Assert.Equal(0m, foreignStockpile.Metal);
        Assert.Equal(0m, foreignStockpile.Crystal);
        Assert.Equal(0m, foreignStockpile.Gas);
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
