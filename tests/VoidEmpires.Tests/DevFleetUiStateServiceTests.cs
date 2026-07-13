using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.Development;
using VoidEmpires.Application.Fleets;
using VoidEmpires.Application.StrategicMap;
using VoidEmpires.Domain.Assets;
using VoidEmpires.Domain.Economy;
using VoidEmpires.Domain.Fleets;
using VoidEmpires.Infrastructure.Development;
using VoidEmpires.Infrastructure.Fleets;
using VoidEmpires.Infrastructure.Persistence;
using VoidEmpires.Infrastructure.StrategicMap;

namespace VoidEmpires.Tests;

public class DevFleetUiStateServiceTests
{
    [Fact]
    public async Task GetAsyncReturnsEmptyUiStateForCivilizationWithNoGroups()
    {
        await using var dbContext = CreateDbContext();
        var civilizationId = Guid.NewGuid();
        var overviewService = new FakeFleetOperationalOverviewService(new GetFleetOperationalOverviewResult(civilizationId, []));

        var result = await new DevFleetUiStateService(
                dbContext,
                overviewService,
                new DevFleetActionManifestService())
            .GetAsync(new GetDevFleetUiStateRequest(civilizationId));

        Assert.Equal(civilizationId, result.CivilizationId);
        Assert.Empty(result.Groups);
        Assert.Empty(result.ResourceContexts);
        Assert.Contains(result.ActionHints, x => x.ActionKey == "fleet.transfer.create");
        Assert.Contains(result.InterceptionNotes, x => x.Note.Contains("not implemented", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task GetAsyncReturnsGroupStateActionHintsAndResourceContext()
    {
        await using var dbContext = CreateDbContext();
        var civilizationId = Guid.NewGuid();
        var currentPlanetId = Guid.NewGuid();
        var groupId = Guid.NewGuid();
        var stockpile = PlanetResourceStockpile.Create(currentPlanetId);
        stockpile.Increase(ResourceType.Credits, 100);
        stockpile.Increase(ResourceType.Metal, 200);
        dbContext.PlanetResourceStockpiles.Add(stockpile);
        await dbContext.SaveChangesAsync();

        var overview = new GetFleetOperationalOverviewResult(
            civilizationId,
            [
                new FleetOperationalGroupDto(
                    groupId,
                    civilizationId,
                    Guid.NewGuid(),
                    currentPlanetId,
                    SpaceAssetType.ScoutCraft,
                    2,
                    OrbitalGroupStatus.Stationed,
                    false,
                    false,
                    null,
                    new FleetOperationalCommandAvailabilityDto(true, true, false, false))
            ]);
        var overviewService = new FakeFleetOperationalOverviewService(overview);

        var result = await new DevFleetUiStateService(
                dbContext,
                overviewService,
                new DevFleetActionManifestService())
            .GetAsync(new GetDevFleetUiStateRequest(civilizationId));

        var group = Assert.Single(result.Groups);
        Assert.Equal(groupId, group.Id);
        Assert.True(group.Commands.CanCreateTransfer);
        Assert.True(group.Commands.CanSplit);
        Assert.True(group.RouteFuelReadiness.CanRequestTravelEstimate);
        Assert.True(group.RouteFuelReadiness.RequiresDestination);
        Assert.Equal("fleet.travel.estimate", group.RouteFuelReadiness.EstimateActionKey);
        Assert.Equal(OrbitalFuelReadinessPolicy.PlaceholderDerived, group.RouteFuelReadiness.FuelReadinessPolicy);
        Assert.Null(group.RouteFuelReadiness.RouteProfile);
        Assert.Null(group.RouteFuelReadiness.FuelReadiness);
        Assert.Contains(group.RouteFuelReadiness.Notes, x => x.Contains("destinationPlanetId", StringComparison.Ordinal));
        Assert.Contains(result.ActionHints, x => x.ActionKey == "fleet.uiState.read" && x.IsReadOnly);
        Assert.Contains(result.ActionHints, x => x.ActionKey == "fleet.travel.estimate" && x.Notes.Contains("fuel readiness", StringComparison.OrdinalIgnoreCase));

        var resourceContext = Assert.Single(result.ResourceContexts);
        Assert.Equal(currentPlanetId, resourceContext.PlanetId);
        Assert.Contains(resourceContext.Balances, x => x.ResourceType == ResourceType.Credits && x.Quantity == 100);
        Assert.Contains(resourceContext.Balances, x => x.ResourceType == ResourceType.Metal && x.Quantity == 200);
    }

    [Fact]
    public async Task GetAsyncDoesNotInventDestinationSpecificRouteFuelEstimate()
    {
        await using var dbContext = CreateDbContext();
        var civilizationId = Guid.NewGuid();
        var groupId = Guid.NewGuid();
        var overview = new GetFleetOperationalOverviewResult(
            civilizationId,
            [
                new FleetOperationalGroupDto(
                    groupId,
                    civilizationId,
                    Guid.NewGuid(),
                    Guid.NewGuid(),
                    SpaceAssetType.CargoCraft,
                    3,
                    OrbitalGroupStatus.Stationed,
                    false,
                    false,
                    null,
                    new FleetOperationalCommandAvailabilityDto(true, true, false, false))
            ]);

        var result = await new DevFleetUiStateService(
                dbContext,
                new FakeFleetOperationalOverviewService(overview),
                new DevFleetActionManifestService())
            .GetAsync(new GetDevFleetUiStateRequest(civilizationId));

        var group = Assert.Single(result.Groups);
        Assert.True(group.RouteFuelReadiness.CanRequestTravelEstimate);
        Assert.Null(group.RouteFuelReadiness.RouteProfile);
        Assert.Null(group.RouteFuelReadiness.FuelReadiness);
        Assert.Contains(group.RouteFuelReadiness.Notes, x => x.Contains("Concrete route profile", StringComparison.Ordinal));
    }

    [Fact]
    public async Task GetAsyncIncludesInterceptionReadinessForActiveOwnTransfer()
    {
        await using var dbContext = CreateDbContext();
        var civilizationId = Guid.NewGuid();
        var transferId = Guid.NewGuid();
        var overview = new GetFleetOperationalOverviewResult(
            civilizationId,
            [
                new FleetOperationalGroupDto(
                    Guid.NewGuid(),
                    civilizationId,
                    Guid.NewGuid(),
                    Guid.NewGuid(),
                    SpaceAssetType.CargoCraft,
                    3,
                    OrbitalGroupStatus.Reserved,
                    false,
                    true,
                    new FleetOperationalTransferDto(
                        transferId,
                        Guid.NewGuid(),
                        2,
                        new DateTime(2026, 6, 1, 10, 0, 0, DateTimeKind.Utc),
                        new DateTime(2026, 6, 1, 12, 0, 0, DateTimeKind.Utc),
                        OrbitalTransferStatus.Planned),
                    new FleetOperationalCommandAvailabilityDto(false, false, false, true))
            ]);

        var result = await new DevFleetUiStateService(
                dbContext,
                new FakeFleetOperationalOverviewService(overview),
                new DevFleetActionManifestService(),
                new FakeInterceptionOpportunityService(
                    new GetInterceptionOpportunitiesResult(
                        civilizationId,
                        [
                            new InterceptionOpportunityDto(
                                transferId,
                                Guid.NewGuid(),
                                Guid.NewGuid(),
                                Guid.NewGuid(),
                                2,
                                new DateTime(2026, 6, 1, 10, 0, 0, DateTimeKind.Utc),
                                new DateTime(2026, 6, 1, 12, 0, 0, DateTimeKind.Utc),
                                OrbitalTransferStatus.Planned,
                                InterceptionOpportunityStatus.ObservedOwnTransfer,
                                [InterceptionOpportunityBlockReason.SelfObservedTransfer],
                                false,
                                "Observed through requesting-civilization fleet state.",
                                "Own active transfers are surfaced as non-hostile readiness metadata only.")
                        ])))
            .GetAsync(new GetDevFleetUiStateRequest(civilizationId));

        var transfer = Assert.Single(result.Groups).ActiveTransfer;
        Assert.NotNull(transfer);
        Assert.NotNull(transfer.InterceptionReadiness);
        Assert.Equal(InterceptionOpportunityStatus.ObservedOwnTransfer, transfer.InterceptionReadiness.OpportunityStatus);
        Assert.Equal([InterceptionOpportunityBlockReason.SelfObservedTransfer], transfer.InterceptionReadiness.BlockReasons);
    }

    [Fact]
    public async Task GetAsyncReturnsNonEmptyGroupsForMinimalValidationSeed()
    {
        await using var dbContext = CreateDbContext();
        await new DevelopmentSeedService(dbContext).ApplyAsync(GetSeedRequest());

        var result = await new DevFleetUiStateService(
                dbContext,
                new FleetOperationalOverviewService(dbContext),
                new DevFleetActionManifestService(),
                new InterceptionOpportunityService(
                    dbContext,
                    new MapVisibilityService(dbContext),
                    new DetectionCoverageService(dbContext, new SensorProfileService(dbContext)),
                    new FleetOperationalOverviewService(dbContext)))
            .GetAsync(new GetDevFleetUiStateRequest(Guid.Parse("00000000-0000-0000-0000-000000000001")));

        Assert.NotEmpty(result.Groups);
        Assert.Contains(result.Groups, x => x.Commands.CanCreateTransfer && x.Status == OrbitalGroupStatus.Stationed);
        Assert.Contains(result.Groups, x => x.HasActiveTransfer && x.ActiveTransfer is not null);
        Assert.NotEmpty(result.ResourceContexts);
        Assert.Contains(result.ResourceContexts, x => x.Balances.Any(balance => balance.ResourceType == ResourceType.Credits && balance.Quantity == 125));
    }

    [Fact]
    public async Task GetAsyncReturnsRicherFleetStateForFleetValidationSeed()
    {
        await using var dbContext = CreateDbContext();
        await new DevelopmentSeedService(dbContext).ApplyAsync(new ApplyDevelopmentSeedRequest("fleet-validation"));

        var service = new DevFleetUiStateService(
            dbContext,
            new FleetOperationalOverviewService(dbContext),
            new DevFleetActionManifestService(),
            new InterceptionOpportunityService(
                dbContext,
                new MapVisibilityService(dbContext),
                new DetectionCoverageService(dbContext, new SensorProfileService(dbContext)),
                new FleetOperationalOverviewService(dbContext)));

        var ownedPlanetId = Guid.Parse("40000000-0000-0000-0000-000000000001");
        var result = await service.GetAsync(new GetDevFleetUiStateRequest(Guid.Parse("00000000-0000-0000-0000-000000000001"), ownedPlanetId));

        Assert.True(result.Groups.Count >= 5);
        Assert.True(result.Groups.Count(x => x.Status == OrbitalGroupStatus.Stationed) >= 3);
        Assert.True(result.Groups.Count(x => x.HasActiveTransfer && x.ActiveTransfer is not null) >= 2);
        Assert.Contains(result.Groups, x => x.AssetType == SpaceAssetType.CargoCraft && x.Status == OrbitalGroupStatus.Stationed);
        Assert.Contains(result.Groups, x => x.ActiveTransfer is not null && x.ActiveTransfer.ArrivalAtUtc == new DateTime(2026, 6, 1, 10, 0, 0, DateTimeKind.Utc));
        Assert.Contains(result.Groups, x => x.CurrentPlanetId == Guid.Parse("40000000-0000-0000-0000-000000000003"));
        Assert.Contains(result.ResourceContexts, x => x.PlanetId == Guid.Parse("40000000-0000-0000-0000-000000000001") && x.Balances.Any(balance => balance.ResourceType == ResourceType.Gas && balance.Quantity == 100));
    }

    [Fact]
    public async Task GetAsyncReturnsCrossCockpitSmokeStateForCockpitValidationSeed()
    {
        await using var dbContext = CreateDbContext();
        await new DevelopmentSeedService(dbContext).ApplyAsync(new ApplyDevelopmentSeedRequest("cockpit-validation"));

        var service = new DevFleetUiStateService(
            dbContext,
            new FleetOperationalOverviewService(dbContext),
            new DevFleetActionManifestService(),
            new InterceptionOpportunityService(
                dbContext,
                new MapVisibilityService(dbContext),
                new DetectionCoverageService(dbContext, new SensorProfileService(dbContext)),
                new FleetOperationalOverviewService(dbContext)));

        var ownedPlanetId = Guid.Parse("40000000-0000-0000-0000-000000000001");
        var result = await service.GetAsync(new GetDevFleetUiStateRequest(Guid.Parse("00000000-0000-0000-0000-000000000001"), ownedPlanetId));

        Assert.True(result.Groups.Count >= 4);
        Assert.Contains(result.Groups, x => x.Status == OrbitalGroupStatus.Stationed && x.Commands.CanCreateTransfer);
        Assert.Contains(result.Groups, x => x.HasActiveTransfer && x.ActiveTransfer is not null);
        Assert.Contains(result.ResourceContexts, x => x.PlanetId == Guid.Parse("40000000-0000-0000-0000-000000000001") && x.Balances.Any(balance => balance.ResourceType == ResourceType.Gas && balance.Quantity == 120));
        Assert.Equal(ownedPlanetId, result.SelectedPlanetId);
        Assert.Contains(result.Planets, x => x.PlanetId == ownedPlanetId && x.IsOwnedByRequestingCivilization);
        Assert.NotEmpty(result.LocalStock);
    }

    [Fact]
    public async Task ReapplyingFleetValidationSeedDoesNotDuplicateExtraTransferScenario()
    {
        await using var dbContext = CreateDbContext();
        var service = new DevelopmentSeedService(dbContext);

        _ = await service.ApplyAsync(new ApplyDevelopmentSeedRequest("fleet-validation"));
        _ = await service.ApplyAsync(new ApplyDevelopmentSeedRequest("fleet-validation"));

        var civilizationId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var ownedPlanetId = Guid.Parse("40000000-0000-0000-0000-000000000001");
        var icePlanetId = Guid.Parse("40000000-0000-0000-0000-000000000003");

        Assert.Equal(1, await dbContext.Set<OrbitalGroup>().CountAsync(x =>
            x.CivilizationId == civilizationId &&
            x.OriginPlanetId == ownedPlanetId &&
            x.CurrentPlanetId == ownedPlanetId &&
            x.AssetType == SpaceAssetType.CargoCraft &&
            x.Quantity == 1 &&
            x.Status == OrbitalGroupStatus.Reserved));
        Assert.Equal(1, await dbContext.Set<OrbitalGroup>().CountAsync(x =>
            x.CivilizationId == civilizationId &&
            x.OriginPlanetId == ownedPlanetId &&
            x.CurrentPlanetId == ownedPlanetId &&
            x.AssetType == SpaceAssetType.CargoCraft &&
            x.Quantity == 1 &&
            x.Status == OrbitalGroupStatus.Stationed));
        Assert.Equal(1, await dbContext.Set<OrbitalTransfer>().CountAsync(x =>
            x.CivilizationId == civilizationId &&
            x.OriginPlanetId == ownedPlanetId &&
            x.DestinationPlanetId == icePlanetId &&
            x.Status == OrbitalTransferStatus.Planned));
    }

    private sealed class FakeFleetOperationalOverviewService(GetFleetOperationalOverviewResult result) : IFleetOperationalOverviewService
    {
        public Task<GetFleetOperationalOverviewResult> GetAsync(
            GetFleetOperationalOverviewRequest request,
            CancellationToken cancellationToken = default) => Task.FromResult(result);
    }

    private sealed class FakeInterceptionOpportunityService(GetInterceptionOpportunitiesResult result) : IInterceptionOpportunityService
    {
        public Task<GetInterceptionOpportunitiesResult> GetAsync(
            GetInterceptionOpportunitiesRequest request,
            CancellationToken cancellationToken = default) => Task.FromResult(result);
    }

    private static ApplyDevelopmentSeedRequest GetSeedRequest() => new("minimal-validation");

    private static VoidEmpiresDbContext CreateDbContext() =>
        new(new DbContextOptionsBuilder<VoidEmpiresDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);
}
