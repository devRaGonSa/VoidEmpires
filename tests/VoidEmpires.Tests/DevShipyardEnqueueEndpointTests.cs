using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.Development;
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
        await using var dbContext = CreateSeededDbContext(databaseName, context =>
        {
            var stockpile = context.PlanetResourceStockpiles.Single(x => x.PlanetId == SeedOwnedPlanetId);
            stockpile.Increase(VoidEmpires.Domain.Economy.ResourceType.Metal, 300);
            stockpile.Increase(VoidEmpires.Domain.Economy.ResourceType.Crystal, 200);
            stockpile.Increase(VoidEmpires.Domain.Economy.ResourceType.Gas, 100);
        });
        using var client = CreateConfiguredClient(databaseName);

        using var enqueueResponse = await client.PostAsJsonAsync("/api/dev/assets/production/enqueue", new
        {
            civilizationId = SeedCivilizationId,
            planetId = SeedOwnedPlanetId,
            target = 2,
            spaceAssetType = 1,
            quantity = 1,
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
        Assert.Single(followUpPayload.UiState.Shipyard.Queue);
        Assert.False(followUpPayload.UiState.Shipyard.ActionSummary.EnqueueSupported);
        Assert.Contains(
            followUpPayload.UiState.Shipyard.Catalog,
            item => item.AssetType == 1 && item.AvailabilityReason == "OpenProductionOrderExists");
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
        ShipyardActionSummaryPayload ActionSummary,
        IReadOnlyList<ShipyardQueueItemPayload> Queue,
        IReadOnlyList<ShipyardCatalogItemPayload> Catalog);

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

    private sealed record ShipyardQueueItemPayload(Guid OrderId, int Quantity, int Sequence, bool IsDue);

    private sealed record ShipyardCatalogItemPayload(int AssetType, string AvailabilityStatus, string AvailabilityReason);
}
