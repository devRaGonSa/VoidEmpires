using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.Gameplay;
using VoidEmpires.Domain.Assets;
using VoidEmpires.Domain.Buildings;
using VoidEmpires.Domain.Colonization;
using VoidEmpires.Domain.Research;
using VoidEmpires.Infrastructure.Gameplay;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Tests;

public class GameplayQueueMaterializationServiceTests
{
    [Fact]
    public async Task MaterializeDueAsyncCountsOnlyScopedOpenQueues()
    {
        await using var dbContext = CreateDbContext();
        var civilizationId = Guid.NewGuid();
        var otherCivilizationId = Guid.NewGuid();
        var planetId = Guid.NewGuid();
        var otherPlanetId = Guid.NewGuid();
        var nowUtc = new DateTime(2026, 6, 15, 12, 0, 0, DateTimeKind.Utc);
        var dueConstruction = CreateConstructionOrder(planetId, 1, nowUtc.AddMinutes(-1), ConstructionQueueItemStatus.Active);
        var notDueConstruction = CreateConstructionOrder(planetId, 2, nowUtc.AddMinutes(5), ConstructionQueueItemStatus.Pending);
        var unrelatedConstruction = CreateConstructionOrder(otherPlanetId, 1, nowUtc.AddMinutes(-1), ConstructionQueueItemStatus.Active);

        dbContext.PlanetOwnerships.Add(PlanetOwnership.Create(planetId, civilizationId));
        dbContext.PlanetOwnerships.Add(PlanetOwnership.Create(otherPlanetId, otherCivilizationId));
        dbContext.PlanetConstructionOrders.AddRange(dueConstruction, notDueConstruction, unrelatedConstruction);
        dbContext.ResearchOrders.Add(CreateResearchOrder(civilizationId, planetId, 1, nowUtc.AddMinutes(-1), ResearchQueueItemStatus.Active));
        dbContext.ResearchOrders.Add(CreateResearchOrder(civilizationId, planetId, 2, nowUtc.AddMinutes(5), ResearchQueueItemStatus.Pending));
        dbContext.ResearchOrders.Add(CreateResearchOrder(otherCivilizationId, otherPlanetId, 1, nowUtc.AddMinutes(-1), ResearchQueueItemStatus.Active));
        dbContext.Set<AssetProductionOrder>().Add(CreateAssetOrder(planetId, 1, nowUtc.AddMinutes(-1), AssetProductionOrderStatus.Active));
        dbContext.Set<AssetProductionOrder>().Add(CreateAssetOrder(planetId, 2, nowUtc.AddMinutes(5), AssetProductionOrderStatus.Pending));
        dbContext.Set<AssetProductionOrder>().Add(CreateAssetOrder(otherPlanetId, 1, nowUtc.AddMinutes(-1), AssetProductionOrderStatus.Active));
        await dbContext.SaveChangesAsync();

        var result = await new GameplayQueueMaterializationService(dbContext).MaterializeDueAsync(
            new MaterializeGameplayQueuesRequest(civilizationId, planetId, nowUtc, true, true, true));

        Assert.True(result.Succeeded);
        Assert.Equal(new QueueMaterializationSummary(1, 1, 1), result.Construction);
        Assert.Equal(new QueueMaterializationSummary(0, 1, 1), result.Research);
        Assert.Equal(new QueueMaterializationSummary(0, 1, 1), result.Shipyard);
        Assert.Equal(ConstructionQueueItemStatus.Completed, dueConstruction.Status);
        Assert.Equal(ConstructionQueueItemStatus.Pending, notDueConstruction.Status);
        Assert.Equal(ConstructionQueueItemStatus.Active, unrelatedConstruction.Status);
        Assert.Contains(dbContext.PlanetBuildings, x => x.PlanetId == planetId && x.BuildingType == BuildingType.MetalMine);
    }

    [Fact]
    public async Task MaterializeDueAsyncDoesNotRepeatConstructionUpgradeAndNotesMissingTargets()
    {
        await using var dbContext = CreateDbContext();
        var civilizationId = Guid.NewGuid();
        var planetId = Guid.NewGuid();
        var nowUtc = new DateTime(2026, 6, 15, 12, 0, 0, DateTimeKind.Utc);
        var building = PlanetBuilding.Create(planetId, BuildingType.MetalMine, 1, 5);
        var upgrade = PlanetConstructionOrder.Create(planetId, ConstructionQueueItemAction.Upgrade, BuildingType.MetalMine, 2, 1, nowUtc.AddMinutes(-6), nowUtc.AddMinutes(-1), ConstructionQueueItemStatus.Active);
        var missing = PlanetConstructionOrder.Create(planetId, ConstructionQueueItemAction.Upgrade, BuildingType.SolarPlant, 2, 2, nowUtc.AddMinutes(-6), nowUtc.AddMinutes(-1), ConstructionQueueItemStatus.Active);

        dbContext.PlanetOwnerships.Add(PlanetOwnership.Create(planetId, civilizationId));
        dbContext.PlanetBuildings.Add(building);
        dbContext.PlanetConstructionOrders.AddRange(upgrade, missing);
        await dbContext.SaveChangesAsync();

        var service = new GameplayQueueMaterializationService(dbContext);
        var request = new MaterializeGameplayQueuesRequest(civilizationId, planetId, nowUtc, true, false, false);
        var first = await service.MaterializeDueAsync(request);
        var second = await service.MaterializeDueAsync(request);

        Assert.Equal(new QueueMaterializationSummary(1, 2, 0), first.Construction);
        Assert.Equal(new QueueMaterializationSummary(0, 1, 0), second.Construction);
        Assert.Equal(2, building.Level);
        Assert.Equal(ConstructionQueueItemStatus.Completed, upgrade.Status);
        Assert.Equal(ConstructionQueueItemStatus.Active, missing.Status);
        Assert.Contains(first.Notes, x => x.Contains("target building was not found", StringComparison.Ordinal));
    }

    private static PlanetConstructionOrder CreateConstructionOrder(Guid planetId, int sequence, DateTime endsAtUtc, ConstructionQueueItemStatus status)
        => PlanetConstructionOrder.Create(planetId, ConstructionQueueItemAction.Construct, BuildingType.MetalMine, 1, sequence, endsAtUtc.AddMinutes(-5), endsAtUtc, status);

    private static ResearchOrder CreateResearchOrder(Guid civilizationId, Guid sourcePlanetId, int sequence, DateTime endsAtUtc, ResearchQueueItemStatus status)
        => ResearchOrder.Create(civilizationId, sourcePlanetId, ResearchType.PlanetaryEngineering, 1, sequence, endsAtUtc.AddMinutes(-10), endsAtUtc, status);

    private static AssetProductionOrder CreateAssetOrder(Guid planetId, int sequence, DateTime endsAtUtc, AssetProductionOrderStatus status)
        => AssetProductionOrder.Create(planetId, AssetProductionTarget.Orbital, null, SpaceAssetType.ScoutCraft, 1, sequence, endsAtUtc.AddMinutes(-3), endsAtUtc, status);

    private static VoidEmpiresDbContext CreateDbContext() =>
        new(new DbContextOptionsBuilder<VoidEmpiresDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);
}
