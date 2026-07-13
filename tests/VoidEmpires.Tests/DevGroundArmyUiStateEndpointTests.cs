using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VoidEmpires.Application.Assets;
using VoidEmpires.Application.Development;
using VoidEmpires.Application.Planets;
using VoidEmpires.Domain.Assets;
using VoidEmpires.Domain.Buildings;
using VoidEmpires.Domain.Economy;
using VoidEmpires.Infrastructure.Development;
using VoidEmpires.Infrastructure.Assets;
using VoidEmpires.Infrastructure.Persistence;
using VoidEmpires.Infrastructure.Planets;

namespace VoidEmpires.Tests;

public class DevGroundArmyUiStateEndpointTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    private static readonly Guid SeedCivilizationId = Guid.Parse("00000000-0000-0000-0000-000000000001");
    private static readonly Guid SeedOwnedPlanetId = Guid.Parse("40000000-0000-0000-0000-000000000001");
    private static readonly Guid SeedOuterPlanetId = Guid.Parse("40000000-0000-0000-0000-000000000002");

    [Fact] public async Task GroundArmyUiStateReturnsNotFoundOutsideDevelopmentByDefault()
    {
        using var client = factory.WithWebHostBuilder(builder => builder.UseEnvironment("Production")).CreateClient();
        using var response = await client.GetAsync($"/api/dev/ground-army/ui-state?civilizationId={SeedCivilizationId}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact] public async Task GroundArmyUiStateReturnsServiceUnavailableWhenPersistenceIsNotConfigured()
    {
        using var client = factory.CreateClientWithPersistenceDisabled();
        using var response = await client.GetAsync($"/api/dev/ground-army/ui-state?civilizationId={SeedCivilizationId}");
        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
    }

    [Fact] public async Task GroundArmyUiStateReturnsBadRequestWhenCivilizationIdIsMissing()
    {
        using var client = CreateConfiguredClient(CreateSeededDbContext());
        using var response = await client.GetAsync("/api/dev/ground-army/ui-state");
        var payload = await response.Content.ReadFromJsonAsync<DevGroundArmyUiStateResponse>();
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(payload);
        Assert.Contains("Civilization id is required.", payload.Errors);
    }

    [Fact] public async Task GroundArmyUiStateReturnsNotFoundForUnknownPlanet()
    {
        using var client = CreateConfiguredClient(CreateSeededDbContext());
        using var response = await client.GetAsync($"/api/dev/ground-army/ui-state?civilizationId={SeedCivilizationId}&planetId={Guid.NewGuid()}");
        var payload = await response.Content.ReadFromJsonAsync<DevGroundArmyUiStateResponse>();
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(payload);
        Assert.Contains("Planet was not found.", payload.Errors);
    }

    [Fact] public async Task GroundArmyUiStateReturnsProductizedAggregateWithoutCompletedHistory()
    {
        await using var dbContext = CreateSeededDbContext(profile: "cockpit-validation");
        var initialQueueCount = await dbContext.Set<AssetProductionOrder>().CountAsync();
        var initialStockCount = await dbContext.Set<PlanetaryAssetStock>().CountAsync();
        using var client = CreateConfiguredClient(dbContext);
        using var response = await client.GetAsync($"/api/dev/ground-army/ui-state?civilizationId={SeedCivilizationId}&planetId={SeedOwnedPlanetId}");
        var payload = await response.Content.ReadFromJsonAsync<DevGroundArmyUiStateResponse>();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload?.UiState?.GroundArmy);
        Assert.Equal("Aurelia", payload.UiState.GroundArmy.PlanetName);
        Assert.True(payload.UiState.GroundArmy.IsOwnedByRequestingCivilization);
        Assert.NotEmpty(payload.UiState.GroundArmy.ResourceStockpile);
        Assert.NotNull(payload.UiState.GroundArmy.Population);
        Assert.Equal(4, payload.UiState.GroundArmy.Catalog.Count);
        Assert.Contains(payload.UiState.GroundArmy.GroundStructures, x => x.BuildingType == BuildingType.Barracks);
        Assert.Equal(1, payload.UiState.GroundArmy.ReadinessSummary.AvailableOptionCount);
        Assert.Equal(3, payload.UiState.GroundArmy.ReadinessSummary.BlockedOptionCount);
        Assert.Empty(payload.UiState.GroundArmy.Queue);
        Assert.Equal(0, payload.UiState.GroundArmy.ReadinessSummary.QueueItemCount);
        Assert.Contains(payload.UiState.GroundArmy.Catalog, x => x.AssetType == PlanetaryAssetType.PatrolGroup.ToString() && x.AvailabilityStatus == "Available");
        Assert.Contains(payload.UiState.GroundArmy.Catalog, x => x.AvailabilityStatus == "Blocked");
        Assert.Equal(initialQueueCount, await dbContext.Set<AssetProductionOrder>().CountAsync());
        Assert.Equal(initialStockCount, await dbContext.Set<PlanetaryAssetStock>().CountAsync());
    }

    [Fact] public async Task GroundArmyUiStateReturnsAvailableAndBlockedOptionsWhenLocalPrerequisitesExist()
    {
        await using var dbContext = CreateSeededDbContext(profile: "cockpit-validation");
        using var client = CreateConfiguredClient(dbContext);
        using var response = await client.GetAsync($"/api/dev/ground-army/ui-state?civilizationId={SeedCivilizationId}&planetId={SeedOwnedPlanetId}");
        var payload = await response.Content.ReadFromJsonAsync<DevGroundArmyUiStateResponse>();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload?.UiState?.GroundArmy);
        Assert.Contains(payload.UiState.GroundArmy.Catalog, x => x.AssetType == PlanetaryAssetType.PatrolGroup.ToString() && x.AvailabilityStatus == "Available");
        Assert.Contains(payload.UiState.GroundArmy.Catalog, x => x.AvailabilityStatus == "Blocked");
        Assert.True(payload.UiState.GroundArmy.ActionSummary.EnqueueSupported);
    }

    [Fact]
    public async Task GroundArmyQueueExcludesDefenseAndClosedOrders()
    {
        var nowUtc = DateTime.UtcNow;
        await using var dbContext = CreateSeededDbContext(context =>
        {
            context.Add(AssetProductionOrder.Create(SeedOwnedPlanetId, AssetProductionTarget.Planetary, PlanetaryAssetType.MissileBattery, null, 1, 90, nowUtc, nowUtc.AddHours(1), AssetProductionOrderStatus.Active));
            context.Add(AssetProductionOrder.Create(SeedOwnedPlanetId, AssetProductionTarget.Planetary, PlanetaryAssetType.VehicleGroup, null, 1, 91, nowUtc, nowUtc.AddHours(1), AssetProductionOrderStatus.Completed));
            context.Add(AssetProductionOrder.Create(SeedOwnedPlanetId, AssetProductionTarget.Planetary, PlanetaryAssetType.SupportGroup, null, 1, 92, nowUtc, nowUtc.AddHours(1), AssetProductionOrderStatus.Cancelled));
        }, "cockpit-validation");
        using var client = CreateConfiguredClient(dbContext);

        var payload = await client.GetFromJsonAsync<DevGroundArmyUiStateResponse>($"/api/dev/ground-army/ui-state?civilizationId={SeedCivilizationId}&planetId={SeedOwnedPlanetId}");

        Assert.NotNull(payload?.UiState?.GroundArmy);
        Assert.Empty(payload.UiState.GroundArmy.Queue);
        Assert.Contains(payload.UiState.GroundArmy.Catalog, x => x.AssetType == nameof(PlanetaryAssetType.PatrolGroup) && x.AvailabilityStatus == "Available");
    }

    [Fact]
    public async Task OpenGroundOrderBlocksGroundCatalogAndRemainsActive()
    {
        var startsAtUtc = DateTime.UtcNow;
        await using var dbContext = CreateSeededDbContext(context =>
            context.Add(AssetProductionOrder.Create(SeedOwnedPlanetId, AssetProductionTarget.Planetary, PlanetaryAssetType.PatrolGroup, null, 2, 93, startsAtUtc, startsAtUtc.AddMinutes(6), AssetProductionOrderStatus.Active)),
            "cockpit-validation");
        using var client = CreateConfiguredClient(dbContext);

        var payload = await client.GetFromJsonAsync<DevGroundArmyUiStateResponse>($"/api/dev/ground-army/ui-state?civilizationId={SeedCivilizationId}&planetId={SeedOwnedPlanetId}");

        Assert.NotNull(payload?.UiState?.GroundArmy);
        var order = Assert.Single(payload.UiState.GroundArmy.Queue);
        Assert.Equal(2, order.Quantity);
        Assert.Equal(startsAtUtc.AddMinutes(6), order.EndsAtUtc);
        Assert.All(payload.UiState.GroundArmy.Catalog, option => Assert.Equal("Blocked", option.AvailabilityStatus));
    }

    [Fact]
    public async Task DueGroundOrderIsMaterializedBeforeUiStateReturns()
    {
        var nowUtc = DateTime.UtcNow;
        var order = AssetProductionOrder.Create(SeedOwnedPlanetId, AssetProductionTarget.Planetary, PlanetaryAssetType.SupportGroup, null, 3, 94, nowUtc.AddMinutes(-4), nowUtc.AddMinutes(-1), AssetProductionOrderStatus.Active);
        await using var dbContext = CreateSeededDbContext(context => context.Add(order), "cockpit-validation");
        var quantityBefore = await dbContext.Set<PlanetaryAssetStock>()
            .Where(x => x.PlanetId == SeedOwnedPlanetId && x.AssetType == PlanetaryAssetType.SupportGroup)
            .Select(x => (int?)x.Quantity)
            .SingleOrDefaultAsync() ?? 0;
        using var client = CreateConfiguredClient(dbContext);

        var payload = await client.GetFromJsonAsync<DevGroundArmyUiStateResponse>($"/api/dev/ground-army/ui-state?civilizationId={SeedCivilizationId}&planetId={SeedOwnedPlanetId}");
        await dbContext.Entry(order).ReloadAsync();
        Assert.Equal(AssetProductionOrderStatus.Completed, order.Status);
        var stock = await dbContext.Set<PlanetaryAssetStock>().SingleAsync(x => x.PlanetId == SeedOwnedPlanetId && x.AssetType == PlanetaryAssetType.SupportGroup);

        Assert.NotNull(payload?.UiState?.GroundArmy);
        Assert.Empty(payload.UiState.GroundArmy.Queue);
        Assert.Equal(quantityBefore + 3, stock.Quantity);
        Assert.Contains(payload.UiState.GroundArmy.Garrison, x => x.AssetType == nameof(PlanetaryAssetType.SupportGroup) && x.Quantity == quantityBefore + 3);
    }

    [Fact]
    public async Task GroundTrainingEnqueuePreservesQuantityDurationAndSpendsResources()
    {
        await using var dbContext = CreateSeededDbContext(profile: "cockpit-validation");
        var stockpile = await dbContext.PlanetResourceStockpiles.SingleAsync(x => x.PlanetId == SeedOwnedPlanetId);
        var metalBefore = stockpile.Metal;
        var crystalBefore = stockpile.Crystal;
        var requestedAtUtc = DateTime.UtcNow;
        using var client = CreateConfiguredClient(dbContext);

        using var response = await client.PostAsJsonAsync("/api/dev/assets/production/enqueue", new
        {
            civilizationId = SeedCivilizationId,
            planetId = SeedOwnedPlanetId,
            target = AssetProductionTarget.Planetary,
            planetaryAssetType = PlanetaryAssetType.PatrolGroup,
            quantity = 2,
            requestedAtUtc
        });
        var payload = await response.Content.ReadFromJsonAsync<EnqueueAssetProductionResponse>();
        Assert.NotNull(payload);
        await dbContext.Entry(stockpile).ReloadAsync();
        var order = await dbContext.Set<AssetProductionOrder>().SingleAsync(x => x.Id == payload.OrderId);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.True(payload.Succeeded);
        Assert.Equal(2, order.Quantity);
        Assert.Equal(requestedAtUtc, order.StartsAtUtc);
        Assert.Equal(requestedAtUtc.AddMinutes(6), order.EndsAtUtc);
        Assert.Equal(metalBefore - 200, stockpile.Metal);
        Assert.Equal(crystalBefore - 50, stockpile.Crystal);
    }

    [Fact] public async Task GroundArmyUiStateAllowsForeignPlanetSelectionWhileKeepingManagementDataHidden()
    {
        using var client = CreateConfiguredClient(CreateSeededDbContext());
        using var response = await client.GetAsync($"/api/dev/ground-army/ui-state?civilizationId={SeedCivilizationId}&planetId={SeedOuterPlanetId}");
        var payload = await response.Content.ReadFromJsonAsync<DevGroundArmyUiStateResponse>();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload?.UiState?.GroundArmy);
        Assert.False(payload.UiState.GroundArmy.IsOwnedByRequestingCivilization);
        Assert.Empty(payload.UiState.GroundArmy.Catalog);
        Assert.Empty(payload.UiState.GroundArmy.Garrison);
    }

    private HttpClient CreateConfiguredClient(VoidEmpiresDbContext dbContext) =>
        factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Development");
            builder.ConfigureAppConfiguration((_, configurationBuilder) => configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?> { ["ConnectionStrings:DefaultConnection"] = "Host=localhost;Database=voidempires_ground_army_ui_state_tests" }));
            builder.ConfigureTestServices(services =>
            {
                services.AddSingleton(dbContext);
                var planetUiStateService = new DevPlanetUiStateService(dbContext);
                services.AddSingleton<IDevPlanetUiStateService>(planetUiStateService);
                services.AddSingleton<IDevGroundArmyUiStateService>(new DevGroundArmyUiStateService(planetUiStateService, dbContext));
                services.AddSingleton<IAssetOrderProcessor>(new AssetOrderProcessor(dbContext));
                services.AddSingleton<IAssetProductionQueueService>(new AssetProductionQueueService(dbContext));
            });
        }).CreateClient();

    private static VoidEmpiresDbContext CreateSeededDbContext(Action<VoidEmpiresDbContext>? seedOverride = null, string profile = "minimal-validation")
    {
        var dbContext = new VoidEmpiresDbContext(new DbContextOptionsBuilder<VoidEmpiresDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        new DevelopmentSeedService(dbContext).ApplyAsync(new ApplyDevelopmentSeedRequest(profile)).GetAwaiter().GetResult();
        seedOverride?.Invoke(dbContext);
        dbContext.SaveChanges();
        return dbContext;
    }

    private sealed record DevGroundArmyUiStateResponse(bool Succeeded, GetDevGroundArmyUiStateResult? UiState, string[] Errors);
    private sealed record EnqueueAssetProductionResponse(bool Succeeded, Guid? OrderId, DateTime? StartsAtUtc, DateTime? EndsAtUtc, string[] Errors);
}
