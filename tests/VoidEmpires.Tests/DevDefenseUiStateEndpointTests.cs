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
        Assert.Equal(5, payload.UiState.Defenses.Catalog.Count);
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
        Assert.Contains(payload.UiState.Defenses.Catalog, x => x.BuildingType == BuildingType.PlanetaryShield);
        Assert.Single(payload.UiState.Defenses.DefenseStructures);
        Assert.Equal(BuildingType.DefenseGrid, payload.UiState.Defenses.DefenseStructures[0].BuildingType);
        Assert.Equal(5, payload.UiState.Defenses.DefenseOptions.Count);
        var missileBattery = Assert.Single(payload.UiState.Defenses.DefenseOptions, x => x.BuildingType == BuildingType.MissileBattery);
        Assert.Equal(0, missileBattery.CurrentLevel);
        Assert.Equal(1, missileBattery.TargetLevel);
        Assert.Equal("Construir", missileBattery.Display?.ActionLabel);
        Assert.Contains("production", missileBattery.Metadata?.DurationPolicyKey, StringComparison.OrdinalIgnoreCase);
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
        Assert.Equal(5, payload.UiState.Defenses.DefenseOptions.Count);
        Assert.Contains(payload.UiState.Defenses.DefenseOptions, option =>
            option.BuildingType == BuildingType.MissileBattery &&
            option.AvailabilityReason == "MissingRequiredBuilding");
        Assert.Contains(payload.UiState.Defenses.DefenseOptions, option =>
            option.BuildingType == BuildingType.PlanetaryShield &&
            option.AvailabilityStatus == "InsufficientResources");
        Assert.Equal(0, payload.UiState.Defenses.ProtectionSummary.AvailableOptionCount);
        Assert.Equal(5, payload.UiState.Defenses.ProtectionSummary.BlockedOptionCount);
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

    private sealed record DevDefenseUiStateResponse(
        bool Succeeded,
        GetDevDefenseUiStateResult? UiState,
        string[] Errors);
}
