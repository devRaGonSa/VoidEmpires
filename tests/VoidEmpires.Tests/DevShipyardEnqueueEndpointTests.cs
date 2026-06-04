using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VoidEmpires.Application.Development;
using VoidEmpires.Domain.Assets;
using VoidEmpires.Domain.Economy;
using VoidEmpires.Infrastructure.Development;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Tests;

public class DevShipyardEnqueueEndpointTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private static readonly Guid SeedCivilizationId = Guid.Parse("00000000-0000-0000-0000-000000000001");
    private static readonly Guid SeedOwnedPlanetId = Guid.Parse("40000000-0000-0000-0000-000000000001");
    private static readonly Guid SeedOuterPlanetId = Guid.Parse("40000000-0000-0000-0000-000000000002");

    [Fact]
    public async Task EnqueueReturnsBadRequestWhenRequiredIdsAreMissing()
    {
        var databaseName = Guid.NewGuid().ToString("N");
        await using var dbContext = CreateSeededDbContext(databaseName);
        using var client = CreateConfiguredClient(databaseName);

        using var response = await client.PostAsJsonAsync("/api/dev/assets/production/enqueue", new { });
        var payload = await response.Content.ReadFromJsonAsync<ShipyardEnqueueResponse>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload!.Succeeded);
        Assert.Contains("Civilization id is required.", payload.Errors);
        Assert.Contains("Planet id is required.", payload.Errors);
    }

    [Fact]
    public async Task EnqueueReturnsConflictWhenPlanetIsNotOwnedByRequestingCivilization()
    {
        var databaseName = Guid.NewGuid().ToString("N");
        await using var dbContext = CreateSeededDbContext(databaseName);
        using var client = CreateConfiguredClient(databaseName);

        using var response = await client.PostAsJsonAsync("/api/dev/assets/production/enqueue", new
        {
            civilizationId = SeedCivilizationId,
            planetId = SeedOuterPlanetId,
            target = 2,
            spaceAssetType = 1,
            quantity = 1,
            requestedAtUtc = "2026-01-01T12:00:00Z",
        });
        var payload = await response.Content.ReadFromJsonAsync<ShipyardEnqueueResponse>();

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload!.Succeeded);
        Assert.Equal(["Planet is not owned by the requesting civilization."], payload.Errors);
    }

    [Fact]
    public async Task EnqueueReturnsConflictWhenResourcesAreInsufficient()
    {
        var databaseName = Guid.NewGuid().ToString("N");
        await using var dbContext = CreateSeededDbContext(databaseName, context =>
        {
            var stockpile = context.PlanetResourceStockpiles.Single(x => x.PlanetId == SeedOwnedPlanetId);
            stockpile.Spend(0, 50, 30, 20);
        });
        using var client = CreateConfiguredClient(databaseName);

        using var response = await client.PostAsJsonAsync("/api/dev/assets/production/enqueue", new
        {
            civilizationId = SeedCivilizationId,
            planetId = SeedOwnedPlanetId,
            target = 2,
            spaceAssetType = 1,
            quantity = 1,
            requestedAtUtc = "2026-01-01T12:00:00Z",
        });
        var payload = await response.Content.ReadFromJsonAsync<ShipyardEnqueueResponse>();

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload!.Succeeded);
        Assert.Equal(["Insufficient resources."], payload.Errors);
    }

    [Fact]
    public async Task EnqueueReturnsBadRequestWhenOrbitalAssetTypeIsMissing()
    {
        var databaseName = Guid.NewGuid().ToString("N");
        await using var dbContext = CreateSeededDbContext(databaseName);
        using var client = CreateConfiguredClient(databaseName);

        using var response = await client.PostAsJsonAsync("/api/dev/assets/production/enqueue", new
        {
            civilizationId = SeedCivilizationId,
            planetId = SeedOwnedPlanetId,
            target = 2,
            quantity = 1,
            requestedAtUtc = "2026-01-01T12:00:00Z",
        });
        var payload = await response.Content.ReadFromJsonAsync<ShipyardEnqueueResponse>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload!.Succeeded);
        Assert.Contains("Space asset type is required.", payload.Errors);
    }

    [Fact]
    public async Task EnqueueReturnsCreatedAndRefreshesShipyardQueueForSeededSuccessPath()
    {
        var databaseName = Guid.NewGuid().ToString("N");
        using var configuredFactory = factory.WithInMemoryPersistence(databaseName: databaseName);
        using var client = configuredFactory.CreateClient();

        using var seedResponse = await client.PostAsJsonAsync("/api/dev/seeds/apply", new { profile = "cockpit-validation" });
        Assert.Equal(HttpStatusCode.OK, seedResponse.StatusCode);

        using var initialResponse = await client.GetAsync($"/api/dev/shipyard/ui-state?civilizationId={SeedCivilizationId}&planetId={SeedOwnedPlanetId}");
        var initialPayload = await initialResponse.Content.ReadFromJsonAsync<ShipyardUiStateEnvelope>();

        Assert.Equal(HttpStatusCode.OK, initialResponse.StatusCode);
        Assert.NotNull(initialPayload?.UiState?.Shipyard);

        var availableItem = Assert.Single(initialPayload.UiState.Shipyard.Catalog.Where(item => item.AssetType == SpaceAssetType.ScoutCraft));
        Assert.NotNull(availableItem.EnqueueCommand);
        var resourcesBefore = initialPayload.UiState.Shipyard.ResourceStockpile.ToDictionary(x => x.ResourceType, x => x.Quantity);
        var queueCountBefore = initialPayload.UiState.Shipyard.Queue.Count;

        using var enqueueResponse = await client.PostAsJsonAsync(availableItem.EnqueueCommand.Route, new
        {
            civilizationId = availableItem.EnqueueCommand.CivilizationId,
            planetId = availableItem.EnqueueCommand.PlanetId,
            target = availableItem.EnqueueCommand.Target,
            spaceAssetType = availableItem.EnqueueCommand.SpaceAssetType,
            quantity = availableItem.EnqueueCommand.Quantity,
            requestedAtUtc = "2026-01-01T12:00:00Z",
        });
        var enqueuePayload = await enqueueResponse.Content.ReadFromJsonAsync<ShipyardEnqueueResponse>();

        Assert.Equal(HttpStatusCode.Created, enqueueResponse.StatusCode);
        Assert.NotNull(enqueuePayload);
        Assert.True(enqueuePayload!.Succeeded);
        Assert.NotNull(enqueuePayload.OrderId);

        using var followUpResponse = await client.GetAsync($"/api/dev/shipyard/ui-state?civilizationId={SeedCivilizationId}&planetId={SeedOwnedPlanetId}");
        var followUpPayload = await followUpResponse.Content.ReadFromJsonAsync<ShipyardUiStateEnvelope>();

        Assert.Equal(HttpStatusCode.OK, followUpResponse.StatusCode);
        Assert.NotNull(followUpPayload?.UiState?.Shipyard);
        Assert.Equal(queueCountBefore + 1, followUpPayload.UiState.Shipyard.Queue.Count);
        Assert.Contains(
            followUpPayload.UiState.Shipyard.Queue,
            item => item.OrderId == enqueuePayload.OrderId &&
                item.AssetType == availableItem.AssetType &&
                item.Quantity == availableItem.EnqueueCommand.Quantity &&
                item.Status == AssetProductionOrderStatus.Active);
        Assert.False(followUpPayload.UiState.Shipyard.ActionSummary.EnqueueSupported);
        Assert.Contains(
            followUpPayload.UiState.Shipyard.Catalog,
            item => item.AssetType == SpaceAssetType.ScoutCraft && item.AvailabilityReason == "OpenProductionOrderExists");

        foreach (var cost in availableItem.Cost)
        {
            var before = resourcesBefore[cost.ResourceType];
            var after = followUpPayload.UiState.Shipyard.ResourceStockpile.Single(x => x.ResourceType == cost.ResourceType).Quantity;
            Assert.Equal(before - cost.Quantity, after);
        }

        using var scope = configuredFactory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<VoidEmpiresDbContext>();
        var persistedOrder = await dbContext.Set<AssetProductionOrder>()
            .SingleAsync(x => x.Id == enqueuePayload.OrderId!.Value);

        Assert.Equal(SeedOwnedPlanetId, persistedOrder.PlanetId);
        Assert.Equal(AssetProductionTarget.Orbital, persistedOrder.Target);
        Assert.Equal(availableItem.AssetType, persistedOrder.SpaceAssetType);
        Assert.Equal(availableItem.EnqueueCommand.Quantity, persistedOrder.Quantity);
        Assert.Equal(AssetProductionOrderStatus.Active, persistedOrder.Status);
    }

    private HttpClient CreateConfiguredClient(string databaseName) =>
        factory.WithInMemoryPersistence(databaseName: databaseName).CreateClient();

    private static VoidEmpiresDbContext CreateSeededDbContext(string databaseName, Action<VoidEmpiresDbContext>? seedOverride = null)
    {
        var dbContext = new VoidEmpiresDbContext(new DbContextOptionsBuilder<VoidEmpiresDbContext>()
            .UseInMemoryDatabase(databaseName)
            .Options);

        new DevelopmentSeedService(dbContext).ApplyAsync(new ApplyDevelopmentSeedRequest("minimal-validation")).GetAwaiter().GetResult();
        seedOverride?.Invoke(dbContext);
        dbContext.SaveChanges();

        return dbContext;
    }

    private sealed record ShipyardEnqueueResponse(
        bool Succeeded,
        Guid? OrderId,
        DateTime? StartsAtUtc,
        DateTime? EndsAtUtc,
        string[] Errors);

    private sealed record ShipyardUiStateEnvelope(
        bool Succeeded,
        ShipyardUiStatePayload? UiState,
        string[] Errors);

    private sealed record ShipyardUiStatePayload(
        Guid CivilizationId,
        Guid? SelectedPlanetId,
        ShipyardPlanetPayload? Shipyard,
        string[] Errors);

    private sealed record ShipyardPlanetPayload(
        Guid PlanetId,
        string PlanetName,
        IReadOnlyList<ShipyardResourceBalancePayload> ResourceStockpile,
        ShipyardActionSummaryPayload ActionSummary,
        IReadOnlyList<ShipyardQueueItemPayload> Queue,
        IReadOnlyList<ShipyardCatalogItemPayload> Catalog);

    private sealed record ShipyardResourceBalancePayload(
        ResourceType ResourceType,
        decimal Quantity);

    private sealed record ShipyardActionSummaryPayload(
        string QueueActionStatus,
        string QueueActionReason,
        bool EnqueueSupported,
        string EnqueueActionStatus,
        string EnqueueActionReason,
        bool CompleteDueSupported,
        string CompleteDueActionStatus,
        string CompleteDueActionReason,
        int OpenQueueCount,
        int DueQueueCount);

    private sealed record ShipyardQueueItemPayload(
        Guid OrderId,
        SpaceAssetType AssetType,
        int Quantity,
        int Sequence,
        AssetProductionOrderStatus Status,
        bool IsDue);

    private sealed record ShipyardCatalogItemPayload(
        SpaceAssetType AssetType,
        string AvailabilityStatus,
        string AvailabilityReason,
        IReadOnlyList<ShipyardResourceBalancePayload> Cost,
        ShipyardEnqueueCommandPayload? EnqueueCommand);

    private sealed record ShipyardEnqueueCommandPayload(
        string ActionKey,
        string Method,
        string Route,
        Guid CivilizationId,
        Guid PlanetId,
        AssetProductionTarget Target,
        SpaceAssetType SpaceAssetType,
        int Quantity);
}
