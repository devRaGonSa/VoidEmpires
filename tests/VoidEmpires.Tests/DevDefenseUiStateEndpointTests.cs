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
using VoidEmpires.Domain.Assets;
using VoidEmpires.Domain.Buildings;
using VoidEmpires.Domain.Economy;
using VoidEmpires.Infrastructure.Development;
using VoidEmpires.Infrastructure.Persistence;
using VoidEmpires.Infrastructure.Planets;

namespace VoidEmpires.Tests;

public class DevDefenseUiStateEndpointTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private static readonly Guid SeedCivilizationId = Guid.Parse("00000000-0000-0000-0000-000000000001");
    private static readonly Guid SeedOwnedPlanetId = Guid.Parse("40000000-0000-0000-0000-000000000001");
    private static readonly Guid SeedOuterPlanetId = Guid.Parse("40000000-0000-0000-0000-000000000002");

    [Fact]
    public async Task DefenseUiStateReturnsNotFoundOutsideDevelopmentByDefault()
    {
        using var client = factory.WithWebHostBuilder(builder => builder.UseEnvironment("Production")).CreateClient();

        using var response = await client.GetAsync($"/api/dev/defenses/ui-state?civilizationId={SeedCivilizationId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DefenseUiStateReturnsServiceUnavailableWhenPersistenceIsNotConfigured()
    {
        using var client = factory.CreateClientWithPersistenceDisabled();

        using var response = await client.GetAsync($"/api/dev/defenses/ui-state?civilizationId={SeedCivilizationId}");

        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
    }

    [Fact]
    public async Task DefenseUiStateReturnsBadRequestWhenCivilizationIdIsMissing()
    {
        using var client = CreateConfiguredClient(CreateSeededDbContext());

        using var response = await client.GetAsync("/api/dev/defenses/ui-state");
        var payload = await response.Content.ReadFromJsonAsync<DevDefenseUiStateResponse>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload.Succeeded);
        Assert.Contains("Civilization id is required.", payload.Errors);
    }

    [Fact]
    public async Task DefenseUiStateReturnsNotFoundForUnknownPlanet()
    {
        using var client = CreateConfiguredClient(CreateSeededDbContext());

        using var response = await client.GetAsync($"/api/dev/defenses/ui-state?civilizationId={SeedCivilizationId}&planetId={Guid.NewGuid()}");
        var payload = await response.Content.ReadFromJsonAsync<DevDefenseUiStateResponse>();

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload.Succeeded);
        Assert.Contains("Planet was not found.", payload.Errors);
    }

    [Fact]
    public async Task DefenseUiStateReturnsReadOnlyDefenseAggregateForCockpitValidation()
    {
        await using var dbContext = CreateSeededDbContext(profile: "cockpit-validation");
        var initialOrderCount = await dbContext.Set<PlanetConstructionOrder>().CountAsync();
        var initialBuildingCount = await dbContext.Set<PlanetBuilding>().CountAsync();

        using var client = CreateConfiguredClient(dbContext);

        using var response = await client.GetAsync($"/api/dev/defenses/ui-state?civilizationId={SeedCivilizationId}&planetId={SeedOwnedPlanetId}");
        var payload = await response.Content.ReadFromJsonAsync<DevDefenseUiStateResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload?.UiState?.Defenses);
        Assert.True(payload.Succeeded);
        Assert.Equal(SeedOwnedPlanetId, payload.UiState.SelectedPlanetId);
        Assert.Equal("Aurelia", payload.UiState.Defenses.PlanetName);
        Assert.True(payload.UiState.Defenses.IsOwnedByRequestingCivilization);
        Assert.NotEmpty(payload.UiState.Defenses.ResourceStockpile);
        Assert.Equal(6, payload.UiState.Defenses.Catalog.Count);
        var defenseCatalogRow = Assert.Single(
            payload.UiState.Defenses.Catalog,
            x => x.BuildingType == BuildingType.DefenseGrid);
        Assert.Equal(BuildingType.DefenseGrid, defenseCatalogRow.BuildingType);
        Assert.Equal("Malla defensiva", defenseCatalogRow.DisplayName);
        Assert.Equal("Defense", defenseCatalogRow.CategoryKey);
        Assert.Equal("CombatSystemDeferred", defenseCatalogRow.FutureCombatDependencyKey);
        Assert.Contains("NonCombat", defenseCatalogRow.Tags);
        Assert.Contains(payload.UiState.Defenses.Catalog, x => x.BuildingType == BuildingType.MissileBattery);
        Assert.Contains(payload.UiState.Defenses.Catalog, x => x.BuildingType == BuildingType.LaserTurret);
        Assert.Contains(payload.UiState.Defenses.Catalog, x => x.BuildingType == BuildingType.IonCannon);
        Assert.Contains(payload.UiState.Defenses.Catalog, x => x.BuildingType == BuildingType.PlasmaCannon);
        Assert.Contains(payload.UiState.Defenses.Catalog, x => x.BuildingType == BuildingType.PlanetaryShield);
        Assert.Single(payload.UiState.Defenses.DefenseStructures);
        Assert.Equal(BuildingType.DefenseGrid, payload.UiState.Defenses.DefenseStructures[0].BuildingType);
        Assert.Equal(6, payload.UiState.Defenses.DefenseOptions.Count);
        var missileBattery = Assert.Single(payload.UiState.Defenses.DefenseOptions, x => x.BuildingType == BuildingType.MissileBattery);
        Assert.Equal(0, missileBattery.CurrentLevel);
        Assert.Equal(1, missileBattery.TargetLevel);
        Assert.Equal("Construir", missileBattery.Display?.ActionLabel);
        Assert.Contains("production", missileBattery.Metadata?.DurationPolicyKey, StringComparison.OrdinalIgnoreCase);
        var plasmaCannon = Assert.Single(payload.UiState.Defenses.DefenseOptions, x => x.BuildingType == BuildingType.PlasmaCannon);
        Assert.Equal(0, plasmaCannon.CurrentLevel);
        Assert.Equal(1, plasmaCannon.TargetLevel);
        Assert.Contains("production", plasmaCannon.Metadata?.DurationPolicyKey, StringComparison.OrdinalIgnoreCase);
        Assert.Contains(payload.UiState.Defenses.DefenseOptions, x => x.BuildingType == BuildingType.DefenseGrid);
        Assert.Contains(payload.UiState.Defenses.DefenseOptions, x => x.BuildingType == BuildingType.PlanetaryShield);
        Assert.Contains("unit production", payload.UiState.Defenses.Diagnostics.Notes[1], StringComparison.OrdinalIgnoreCase);
        Assert.Equal(initialOrderCount, await dbContext.Set<PlanetConstructionOrder>().CountAsync());
        Assert.Equal(initialBuildingCount, await dbContext.Set<PlanetBuilding>().CountAsync());
        Assert.Empty(await dbContext.Set<AssetProductionOrder>().Where(x => x.Status == AssetProductionOrderStatus.Active).ToListAsync());
    }

    [Fact]
    public async Task DefenseUiStateReturnsBlockedOptionWhenLocalResourcesAreInsufficient()
    {
        await using var dbContext = CreateSeededDbContext(context =>
        {
            var stockpile = context.PlanetResourceStockpiles.Single(x => x.PlanetId == SeedOwnedPlanetId);
            context.Entry(stockpile).Property(x => x.Metal).CurrentValue = 10;
            context.Entry(stockpile).Property(x => x.Crystal).CurrentValue = 5;
        });
        using var client = CreateConfiguredClient(dbContext);

        using var response = await client.GetAsync($"/api/dev/defenses/ui-state?civilizationId={SeedCivilizationId}&planetId={SeedOwnedPlanetId}");
        var payload = await response.Content.ReadFromJsonAsync<DevDefenseUiStateResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload?.UiState?.Defenses);
        Assert.Equal(6, payload.UiState.Defenses.DefenseOptions.Count);
        Assert.Contains(payload.UiState.Defenses.DefenseOptions, option =>
            option.BuildingType == BuildingType.MissileBattery &&
            option.AvailabilityReason == "MissingRequiredBuilding");
        Assert.Contains(payload.UiState.Defenses.DefenseOptions, option =>
            option.BuildingType == BuildingType.PlanetaryShield &&
            option.AvailabilityStatus == "InsufficientResources");
        Assert.Equal(0, payload.UiState.Defenses.ProtectionSummary.AvailableOptionCount);
        Assert.Equal(6, payload.UiState.Defenses.ProtectionSummary.BlockedOptionCount);
    }

    [Fact]
    public async Task DefenseUiStateIgnoresUnrelatedConstructionButBlocksRequiredBuildingWork()
    {
        await using var unrelatedConstructionDbContext = CreateSeededDbContext(context =>
        {
            var stockpile = context.PlanetResourceStockpiles.Single(x => x.PlanetId == SeedOwnedPlanetId);
            stockpile.Increase(ResourceType.Metal, 1_000m);
            stockpile.Increase(ResourceType.Crystal, 500m);
            stockpile.Increase(ResourceType.Gas, 100m);
            AddOpenConstruction(context, BuildingType.MetalMine, 99_001);
        }, profile: "cockpit-validation");
        using var unrelatedClient = CreateConfiguredClient(unrelatedConstructionDbContext);

        using var unrelatedResponse = await unrelatedClient.GetAsync($"/api/dev/defenses/ui-state?civilizationId={SeedCivilizationId}&planetId={SeedOwnedPlanetId}");
        var unrelatedPayload = await unrelatedResponse.Content.ReadFromJsonAsync<DevDefenseUiStateResponse>();

        Assert.Equal(HttpStatusCode.OK, unrelatedResponse.StatusCode);
        Assert.NotNull(unrelatedPayload?.UiState?.Defenses);
        Assert.Contains(unrelatedPayload.UiState.Defenses.DefenseOptions, option =>
            option.BuildingType == BuildingType.MissileBattery &&
            option.AvailabilityStatus == "Available");

        await using var requiredConstructionDbContext = CreateSeededDbContext(context =>
        {
            var stockpile = context.PlanetResourceStockpiles.Single(x => x.PlanetId == SeedOwnedPlanetId);
            stockpile.Increase(ResourceType.Metal, 1_000m);
            stockpile.Increase(ResourceType.Crystal, 500m);
            stockpile.Increase(ResourceType.Gas, 100m);
            AddOpenConstruction(context, BuildingType.DefenseGrid, 99_002);
        }, profile: "cockpit-validation");
        using var requiredClient = CreateConfiguredClient(requiredConstructionDbContext);

        using var requiredResponse = await requiredClient.GetAsync($"/api/dev/defenses/ui-state?civilizationId={SeedCivilizationId}&planetId={SeedOwnedPlanetId}");
        var requiredPayload = await requiredResponse.Content.ReadFromJsonAsync<DevDefenseUiStateResponse>();

        Assert.Equal(HttpStatusCode.OK, requiredResponse.StatusCode);
        Assert.NotNull(requiredPayload?.UiState?.Defenses);
        Assert.Contains(requiredPayload.UiState.Defenses.DefenseOptions, option =>
            option.BuildingType == BuildingType.MissileBattery &&
            option.AvailabilityReason == "RequiredBuildingInConstruction");
    }

    [Fact]
    public async Task DefenseUiStateAllowsForeignPlanetSelectionWhileKeepingManagementDataHidden()
    {
        using var client = CreateConfiguredClient(CreateSeededDbContext());

        using var response = await client.GetAsync($"/api/dev/defenses/ui-state?civilizationId={SeedCivilizationId}&planetId={SeedOuterPlanetId}");
        var payload = await response.Content.ReadFromJsonAsync<DevDefenseUiStateResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload?.UiState?.Defenses);
        Assert.False(payload.UiState.Defenses.IsOwnedByRequestingCivilization);
        Assert.Empty(payload.UiState.Defenses.ResourceStockpile);
        Assert.Empty(payload.UiState.Defenses.DefenseStructures);
        Assert.Empty(payload.UiState.Defenses.DefenseOptions);
        Assert.Contains(payload.UiState.Defenses.Diagnostics.Notes, x => x.Contains("non-owned", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task DefenseProductionEnqueueCreatesPlanetaryUnitOrderAndRefreshesDefenseQueue()
    {
        var databaseName = Guid.NewGuid().ToString("N");
        using var configuredFactory = factory.WithInMemoryPersistence(databaseName: databaseName);
        using var client = configuredFactory.CreateClient();

        using var seedResponse = await client.PostAsJsonAsync("/api/dev/seeds/apply", new { profile = "cockpit-validation" });
        Assert.Equal(HttpStatusCode.OK, seedResponse.StatusCode);

        using (var scope = configuredFactory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<VoidEmpiresDbContext>();
            var stockpile = await dbContext.PlanetResourceStockpiles.SingleAsync(x => x.PlanetId == SeedOwnedPlanetId);
            stockpile.Increase(ResourceType.Metal, 1_000m);
            stockpile.Increase(ResourceType.Crystal, 500m);
            stockpile.Increase(ResourceType.Gas, 100m);
            dbContext.Set<AssetProductionOrder>().Add(AssetProductionOrder.Create(
                SeedOwnedPlanetId,
                AssetProductionTarget.Orbital,
                null,
                SpaceAssetType.ScoutCraft,
                1,
                99_001,
                new DateTime(2026, 12, 1, 12, 0, 0, DateTimeKind.Utc),
                new DateTime(2026, 12, 1, 12, 20, 0, DateTimeKind.Utc),
                AssetProductionOrderStatus.Active));
            await dbContext.SaveChangesAsync();
        }

        using var enqueueResponse = await client.PostAsJsonAsync("/api/dev/assets/production/enqueue", new
        {
            civilizationId = SeedCivilizationId,
            planetId = SeedOwnedPlanetId,
            target = AssetProductionTarget.Planetary,
            planetaryAssetType = PlanetaryAssetType.MissileBattery,
            quantity = 2,
            requestedAtUtc = "2026-01-01T12:00:00Z",
        });

        Assert.Equal(HttpStatusCode.Created, enqueueResponse.StatusCode);

        using var defenseStateResponse = await client.GetAsync($"/api/dev/defenses/ui-state?civilizationId={SeedCivilizationId}&planetId={SeedOwnedPlanetId}");
        var defensePayload = await defenseStateResponse.Content.ReadFromJsonAsync<DevDefenseUiStateResponse>();

        Assert.Equal(HttpStatusCode.OK, defenseStateResponse.StatusCode);
        Assert.NotNull(defensePayload?.UiState?.Defenses);
        var queueItem = Assert.Single(defensePayload.UiState.Defenses.DefenseQueue);
        Assert.Equal(BuildingType.MissileBattery, queueItem.BuildingType);
        Assert.Equal(2, queueItem.TargetLevel);
        Assert.Equal("Construir", queueItem.Display?.ActionLabel);

        using var verificationScope = configuredFactory.Services.CreateScope();
        var verificationDb = verificationScope.ServiceProvider.GetRequiredService<VoidEmpiresDbContext>();
        var order = await verificationDb.Set<AssetProductionOrder>().SingleAsync(x =>
            x.PlanetId == SeedOwnedPlanetId &&
            x.Target == AssetProductionTarget.Planetary &&
            x.PlanetaryAssetType == PlanetaryAssetType.MissileBattery &&
            x.Status == AssetProductionOrderStatus.Active);
        Assert.Equal(2, order.Quantity);
    }

    private HttpClient CreateConfiguredClient(VoidEmpiresDbContext dbContext) =>
        factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Development");
            builder.ConfigureAppConfiguration((_, configurationBuilder) =>
                configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] = "Host=localhost;Database=voidempires_defense_ui_state_tests"
                }));
            builder.ConfigureTestServices(services =>
            {
                var planetUiStateService = new DevPlanetUiStateService(dbContext);
                services.AddSingleton<IDevPlanetUiStateService>(planetUiStateService);
                services.AddSingleton<IDevDefenseUiStateService>(new DevDefenseUiStateService(planetUiStateService, dbContext));
            });
        }).CreateClient();

    private static VoidEmpiresDbContext CreateSeededDbContext(Action<VoidEmpiresDbContext>? seedOverride = null, string profile = "minimal-validation")
    {
        var dbContext = new VoidEmpiresDbContext(new DbContextOptionsBuilder<VoidEmpiresDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

        new DevelopmentSeedService(dbContext).ApplyAsync(new ApplyDevelopmentSeedRequest(profile)).GetAwaiter().GetResult();
        seedOverride?.Invoke(dbContext);
        dbContext.SaveChanges();

        return dbContext;
    }

    private static void AddOpenConstruction(VoidEmpiresDbContext context, BuildingType buildingType, int sequence) =>
        context.PlanetConstructionOrders.Add(PlanetConstructionOrder.Create(SeedOwnedPlanetId, ConstructionQueueItemAction.Upgrade,
            buildingType, 2, sequence, new DateTime(2026, 12, 1, 12, 0, 0, DateTimeKind.Utc),
            new DateTime(2026, 12, 1, 12, 20, 0, DateTimeKind.Utc), ConstructionQueueItemStatus.Active));

    private sealed record DevDefenseUiStateResponse(
        bool Succeeded,
        GetDevDefenseUiStateResult? UiState,
        string[] Errors);
}
