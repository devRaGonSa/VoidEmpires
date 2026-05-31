using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.Fleets;
using VoidEmpires.Domain.Assets;
using VoidEmpires.Domain.Fleets;
using VoidEmpires.Infrastructure.Fleets;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Tests;

public class FleetOperationalOverviewServiceTests
{
    [Fact]
    public async Task GetAsyncReturnsEmptyOverviewForCivilizationWithNoGroups()
    {
        await using var dbContext = CreateDbContext();
        var civilizationId = Guid.NewGuid();

        var result = await new FleetOperationalOverviewService(dbContext)
            .GetAsync(new GetFleetOperationalOverviewRequest(civilizationId));

        Assert.Equal(civilizationId, result.CivilizationId);
        Assert.Empty(result.Groups);
    }

    [Fact]
    public async Task GetAsyncReturnsStationaryGroupWithCommandAvailability()
    {
        await using var dbContext = CreateDbContext();
        var civilizationId = Guid.NewGuid();
        var currentPlanetId = Guid.NewGuid();
        var group = OrbitalGroup.CreateStationed(
            civilizationId,
            Guid.NewGuid(),
            currentPlanetId,
            SpaceAssetType.ScoutCraft,
            2);
        var mergeCandidate = OrbitalGroup.CreateStationed(
            civilizationId,
            Guid.NewGuid(),
            currentPlanetId,
            SpaceAssetType.ScoutCraft,
            1);
        dbContext.Set<OrbitalGroup>().AddRange(group, mergeCandidate);
        await dbContext.SaveChangesAsync();

        var result = await new FleetOperationalOverviewService(dbContext)
            .GetAsync(new GetFleetOperationalOverviewRequest(civilizationId));

        var item = result.Groups.Single(x => x.Id == group.Id);
        Assert.Equal(group.OriginPlanetId, item.OriginPlanetId);
        Assert.Equal(currentPlanetId, item.CurrentPlanetId);
        Assert.Equal(SpaceAssetType.ScoutCraft, item.AssetType);
        Assert.Equal(2, item.Quantity);
        Assert.Equal(OrbitalGroupStatus.Stationed, item.Status);
        Assert.False(item.HasActiveTransfer);
        Assert.Null(item.ActiveTransfer);
        Assert.True(item.Commands.CanCreateTransfer);
        Assert.True(item.Commands.CanSplit);
        Assert.True(item.Commands.CanMerge);
        Assert.False(item.Commands.CanCancelTransfer);
    }

    [Fact]
    public async Task GetAsyncReturnsActiveTransferSummaryAndBlocksGroupCommands()
    {
        await using var dbContext = CreateDbContext();
        var group = CreateReservedGroup(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        var transfer = CreateTransfer(group, Guid.NewGuid());
        dbContext.Set<OrbitalGroup>().Add(group);
        dbContext.Set<OrbitalTransfer>().Add(transfer);
        await dbContext.SaveChangesAsync();

        var result = await new FleetOperationalOverviewService(dbContext)
            .GetAsync(new GetFleetOperationalOverviewRequest(group.CivilizationId));

        var item = Assert.Single(result.Groups);
        Assert.True(item.HasActiveTransfer);
        Assert.NotNull(item.ActiveTransfer);
        Assert.Equal(transfer.Id, item.ActiveTransfer.Id);
        Assert.Equal(transfer.DestinationPlanetId, item.ActiveTransfer.DestinationPlanetId);
        Assert.Equal(transfer.DepartureAtUtc, item.ActiveTransfer.DepartureAtUtc);
        Assert.Equal(transfer.ArrivalAtUtc, item.ActiveTransfer.ArrivalAtUtc);
        Assert.Equal(OrbitalTransferStatus.Planned, item.ActiveTransfer.Status);
        Assert.False(item.Commands.CanCreateTransfer);
        Assert.False(item.Commands.CanSplit);
        Assert.False(item.Commands.CanMerge);
        Assert.True(item.Commands.CanCancelTransfer);
    }

    [Fact]
    public async Task GetAsyncExcludesGroupsFromOtherCivilizations()
    {
        await using var dbContext = CreateDbContext();
        var civilizationId = Guid.NewGuid();
        dbContext.Set<OrbitalGroup>().Add(OrbitalGroup.CreateStationed(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            SpaceAssetType.ScoutCraft,
            1));
        await dbContext.SaveChangesAsync();

        var result = await new FleetOperationalOverviewService(dbContext)
            .GetAsync(new GetFleetOperationalOverviewRequest(civilizationId));

        Assert.Empty(result.Groups);
    }

    private static OrbitalGroup CreateReservedGroup(Guid civilizationId, Guid originPlanetId, Guid currentPlanetId)
    {
        var group = OrbitalGroup.CreateStationed(
            civilizationId,
            originPlanetId,
            currentPlanetId,
            SpaceAssetType.ScoutCraft,
            3);
        group.Reserve();
        return group;
    }

    private static OrbitalTransfer CreateTransfer(OrbitalGroup group, Guid destinationPlanetId) =>
        OrbitalTransfer.CreatePlanned(
            group.CivilizationId,
            group.Id,
            group.CurrentPlanetId,
            destinationPlanetId,
            1,
            new DateTime(2026, 5, 30, 12, 0, 0, DateTimeKind.Utc),
            new DateTime(2026, 5, 30, 13, 0, 0, DateTimeKind.Utc));

    private static VoidEmpiresDbContext CreateDbContext() =>
        new(new DbContextOptionsBuilder<VoidEmpiresDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);
}
