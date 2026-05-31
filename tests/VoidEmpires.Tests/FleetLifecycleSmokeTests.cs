using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.Fleets;
using VoidEmpires.Domain.Assets;
using VoidEmpires.Domain.Economy;
using VoidEmpires.Domain.Fleets;
using VoidEmpires.Domain.Galaxy;
using VoidEmpires.Infrastructure.Economy;
using VoidEmpires.Infrastructure.Fleets;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Tests;

public class FleetLifecycleSmokeTests
{
    [Fact]
    public async Task CurrentFleetLifecycleRemainsCoherentAcrossServices()
    {
        await using var dbContext = CreateDbContext();
        var civilizationId = Guid.NewGuid();
        var originPlanetId = Guid.NewGuid();
        var firstDestinationPlanetId = Guid.NewGuid();
        var finalDestinationPlanetId = Guid.NewGuid();
        var requestedAtUtc = new DateTime(2026, 5, 31, 12, 0, 0, DateTimeKind.Utc);
        var group = OrbitalGroup.CreateStationed(
            civilizationId,
            originPlanetId,
            originPlanetId,
            SpaceAssetType.ScoutCraft,
            4);
        var mergeCandidate = OrbitalGroup.CreateStationed(
            civilizationId,
            Guid.NewGuid(),
            originPlanetId,
            SpaceAssetType.ScoutCraft,
            1);
        var stockpile = PlanetResourceStockpile.Create(originPlanetId);
        stockpile.Increase(ResourceType.Credits, 20);
        stockpile.Increase(ResourceType.Gas, 8);
        dbContext.Set<Planet>().AddRange(
            CreatePlanet(originPlanetId),
            CreatePlanet(firstDestinationPlanetId),
            CreatePlanet(finalDestinationPlanetId));
        dbContext.Set<OrbitalGroup>().AddRange(group, mergeCandidate);
        dbContext.PlanetResourceStockpiles.Add(stockpile);
        await dbContext.SaveChangesAsync();

        var resourceSpendService = new ResourceSpendService(dbContext);
        var estimateService = new OrbitalTravelEstimateService(
            dbContext,
            resourceSpendService,
            new OrbitalRouteProfileService());
        var transferService = new OrbitalTransferPersistenceService(dbContext, resourceSpendService);
        var splitService = new OrbitalGroupSplitService(dbContext);
        var mergeService = new OrbitalGroupMergeService(dbContext);
        var cancelService = new OrbitalTransferCancelService(dbContext);
        var completionService = new OrbitalTransferCompletionService(dbContext);
        var overviewService = new FleetOperationalOverviewService(dbContext);

        var estimate = await estimateService.EstimateAsync(new EstimateOrbitalTravelRequest(
            civilizationId,
            group.Id,
            firstDestinationPlanetId));
        Assert.True(estimate.Succeeded);
        Assert.True(estimate.CanAfford);
        Assert.Contains(estimate.ResourceCosts, x => x.ResourceType == ResourceType.Credits && x.Quantity == 5m);
        Assert.Contains(estimate.ResourceCosts, x => x.ResourceType == ResourceType.Gas && x.Quantity == 2m);

        var firstTransfer = await transferService.PersistAsync(new PersistOrbitalTransferRequest(
            civilizationId,
            group.Id,
            firstDestinationPlanetId,
            requestedAtUtc));
        Assert.True(firstTransfer.Succeeded);
        Assert.Equal(15, (await dbContext.PlanetResourceStockpiles.SingleAsync()).Credits);
        Assert.Equal(6, (await dbContext.PlanetResourceStockpiles.SingleAsync()).Gas);

        var activeSplit = await splitService.SplitAsync(new SplitOrbitalGroupRequest(civilizationId, group.Id, 1));
        Assert.False(activeSplit.Succeeded);
        Assert.Contains("Source orbital group already has an active transfer.", activeSplit.Errors);

        var activeMerge = await mergeService.MergeAsync(new MergeOrbitalGroupsRequest(
            civilizationId,
            group.Id,
            mergeCandidate.Id));
        Assert.False(activeMerge.Succeeded);
        Assert.Contains("Target orbital group already has an active transfer.", activeMerge.Errors);

        var cancel = await cancelService.CancelAsync(new CancelOrbitalTransferRequest(
            civilizationId,
            firstTransfer.OrbitalTransferId!.Value));
        Assert.True(cancel.Succeeded);
        Assert.Equal(OrbitalGroupStatus.Stationed, (await dbContext.Set<OrbitalGroup>().SingleAsync(x => x.Id == group.Id)).Status);

        var split = await splitService.SplitAsync(new SplitOrbitalGroupRequest(civilizationId, group.Id, 2));
        Assert.True(split.Succeeded);
        var merge = await mergeService.MergeAsync(new MergeOrbitalGroupsRequest(
            civilizationId,
            group.Id,
            split.NewOrbitalGroupId!.Value));
        Assert.True(merge.Succeeded);
        Assert.Equal(4, merge.TargetQuantity);

        var secondTransfer = await transferService.PersistAsync(new PersistOrbitalTransferRequest(
            civilizationId,
            group.Id,
            finalDestinationPlanetId,
            requestedAtUtc.AddHours(2)));
        Assert.True(secondTransfer.Succeeded);
        Assert.Equal(10, (await dbContext.PlanetResourceStockpiles.SingleAsync()).Credits);
        Assert.Equal(4, (await dbContext.PlanetResourceStockpiles.SingleAsync()).Gas);

        var completion = await completionService.CompleteDueAsync(secondTransfer.ArrivalAtUtc!.Value);
        Assert.Equal(1, completion.CompletedCount);
        Assert.Contains(secondTransfer.OrbitalTransferId!.Value, completion.CompletedTransferIds);
        Assert.Contains(group.Id, completion.CompletedOrbitalGroupIds);

        var overview = await overviewService.GetAsync(new GetFleetOperationalOverviewRequest(civilizationId));
        var finalGroup = overview.Groups.Single(x => x.Id == group.Id);
        Assert.Equal(finalDestinationPlanetId, finalGroup.CurrentPlanetId);
        Assert.Equal(OrbitalGroupStatus.Stationed, finalGroup.Status);
        Assert.False(finalGroup.HasActiveTransfer);
        Assert.Null(finalGroup.ActiveTransfer);
        Assert.True(finalGroup.Commands.CanCreateTransfer);
        Assert.True(finalGroup.Commands.CanSplit);
        Assert.False(finalGroup.Commands.CanCancelTransfer);
    }

    private static Planet CreatePlanet(Guid id) =>
        new(id, Guid.NewGuid(), "Smoke", 1, PlanetType.Terran, 100);

    private static VoidEmpiresDbContext CreateDbContext() =>
        new(new DbContextOptionsBuilder<VoidEmpiresDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);
}
