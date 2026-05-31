using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.Fleets;
using VoidEmpires.Domain.Assets;
using VoidEmpires.Domain.Economy;
using VoidEmpires.Domain.Fleets;
using VoidEmpires.Infrastructure.Fleets;
using VoidEmpires.Infrastructure.Persistence;

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

    private sealed class FakeFleetOperationalOverviewService(GetFleetOperationalOverviewResult result) : IFleetOperationalOverviewService
    {
        public Task<GetFleetOperationalOverviewResult> GetAsync(
            GetFleetOperationalOverviewRequest request,
            CancellationToken cancellationToken = default) => Task.FromResult(result);
    }

    private static VoidEmpiresDbContext CreateDbContext() =>
        new(new DbContextOptionsBuilder<VoidEmpiresDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);
}
