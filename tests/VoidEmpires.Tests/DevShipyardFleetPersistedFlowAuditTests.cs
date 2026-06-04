using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using VoidEmpires.Domain.Economy;

namespace VoidEmpires.Tests;

public class DevShipyardFleetPersistedFlowAuditTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private static readonly Guid SeedCivilizationId = Guid.Parse("00000000-0000-0000-0000-000000000001");
    private static readonly Guid SeedOwnedPlanetId = Guid.Parse("40000000-0000-0000-0000-000000000001");

    [Fact]
    public async Task ShipyardEnqueueUpdatesShipyardAndSharedFleetResourceReadStateWithoutMutatingFleetGroups()
    {
        var databaseName = Guid.NewGuid().ToString("N");
        using var configuredFactory = factory.WithInMemoryPersistence(databaseName: databaseName);
        using var client = configuredFactory.CreateClient();

        using var seedResponse = await client.PostAsJsonAsync("/api/dev/seeds/apply", new { profile = "cockpit-validation" });
        Assert.Equal(HttpStatusCode.OK, seedResponse.StatusCode);

        var shipyardBefore = await GetShipyardAsync(client);
        var fleetBefore = await GetFleetAsync(client);

        Assert.NotNull(shipyardBefore?.UiState?.Shipyard);
        Assert.NotNull(fleetBefore?.UiState);

        var scoutCraft = shipyardBefore.UiState.Shipyard.Catalog.Single(x => x.AssetType == 1);
        Assert.Equal("Available", scoutCraft.AvailabilityStatus);

        var fleetGroupSnapshot = fleetBefore.UiState.Groups
            .Select(x => new { x.Id, x.CurrentPlanetId, x.Quantity, x.Status, x.HasActiveTransfer })
            .OrderBy(x => x.Id)
            .ToArray();

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

        var shipyardAfter = await GetShipyardAsync(client);
        var fleetAfter = await GetFleetAsync(client);

        Assert.NotNull(shipyardAfter?.UiState?.Shipyard);
        Assert.NotNull(fleetAfter?.UiState);

        Assert.Equal(
            shipyardBefore.UiState.Shipyard.ActionSummary.OpenQueueCount + 1,
            shipyardAfter.UiState.Shipyard.ActionSummary.OpenQueueCount);
        Assert.Equal(
            shipyardBefore.UiState.Shipyard.OrbitalStock,
            shipyardAfter.UiState.Shipyard.OrbitalStock);
        Assert.False(shipyardAfter.UiState.Shipyard.ActionSummary.EnqueueSupported);
        Assert.Contains(
            shipyardAfter.UiState.Shipyard.Catalog,
            x => x.AssetType == 1 && x.AvailabilityReason == "OpenProductionOrderExists");

        var metalBefore = GetResourceQuantity(shipyardBefore.UiState.Shipyard.ResourceStockpile, ResourceType.Metal);
        var metalAfter = GetResourceQuantity(shipyardAfter.UiState.Shipyard.ResourceStockpile, ResourceType.Metal);
        Assert.True(metalAfter < metalBefore);

        var fleetPlanetBefore = fleetBefore.UiState.ResourceContexts.Single(x => x.PlanetId == SeedOwnedPlanetId);
        var fleetPlanetAfter = fleetAfter.UiState.ResourceContexts.Single(x => x.PlanetId == SeedOwnedPlanetId);
        Assert.True(
            GetResourceQuantity(fleetPlanetAfter.Balances, ResourceType.Metal) <
            GetResourceQuantity(fleetPlanetBefore.Balances, ResourceType.Metal));

        var fleetGroupSnapshotAfter = fleetAfter.UiState.Groups
            .Select(x => new { x.Id, x.CurrentPlanetId, x.Quantity, x.Status, x.HasActiveTransfer })
            .OrderBy(x => x.Id)
            .ToArray();
        Assert.Equal(fleetGroupSnapshot, fleetGroupSnapshotAfter);
    }

    private static async Task<ShipyardUiStateEnvelope?> GetShipyardAsync(HttpClient client) =>
        await client.GetFromJsonAsync<ShipyardUiStateEnvelope>(
            $"/api/dev/shipyard/ui-state?civilizationId={SeedCivilizationId}&planetId={SeedOwnedPlanetId}");

    private static async Task<FleetUiStateEnvelope?> GetFleetAsync(HttpClient client) =>
        await client.GetFromJsonAsync<FleetUiStateEnvelope>(
            $"/api/dev/fleets/ui-state?civilizationId={SeedCivilizationId}");

    private static decimal GetResourceQuantity(IEnumerable<ResourceBalancePayload> balances, ResourceType resourceType) =>
        balances.Single(x => x.ResourceType == (int)resourceType).Quantity;

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
        ShipyardPayload? Shipyard,
        string[] Errors);

    private sealed record ShipyardPayload(
        IReadOnlyList<ResourceBalancePayload> ResourceStockpile,
        IReadOnlyList<ShipyardCatalogItemPayload> Catalog,
        IReadOnlyList<ShipyardStockItemPayload> OrbitalStock,
        ShipyardActionSummaryPayload ActionSummary);

    private sealed record ShipyardCatalogItemPayload(
        int AssetType,
        string AvailabilityStatus,
        string AvailabilityReason);

    private sealed record ShipyardStockItemPayload(int AssetType, int Quantity);

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

    private sealed record FleetUiStateEnvelope(
        bool Succeeded,
        FleetUiStatePayload? UiState,
        string[] Errors);

    private sealed record FleetUiStatePayload(
        Guid CivilizationId,
        IReadOnlyList<FleetGroupPayload> Groups,
        IReadOnlyList<FleetResourceContextPayload> ResourceContexts);

    private sealed record FleetGroupPayload(
        Guid Id,
        Guid CurrentPlanetId,
        int Quantity,
        int Status,
        bool HasActiveTransfer);

    private sealed record FleetResourceContextPayload(
        Guid PlanetId,
        IReadOnlyList<ResourceBalancePayload> Balances);

    private sealed record ResourceBalancePayload(
        int ResourceType,
        decimal Quantity);
}
