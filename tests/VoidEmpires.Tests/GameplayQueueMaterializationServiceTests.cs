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

        dbContext.PlanetOwnerships.Add(PlanetOwnership.Create(planetId, civilizationId));
        dbContext.PlanetOwnerships.Add(PlanetOwnership.Create(otherPlanetId, otherCivilizationId));
        dbContext.PlanetConstructionOrders.Add(CreateConstructionOrder(planetId, 1, nowUtc.AddMinutes(-1), ConstructionQueueItemStatus.Active));
        dbContext.PlanetConstructionOrders.Add(CreateConstructionOrder(planetId, 2, nowUtc.AddMinutes(5), ConstructionQueueItemStatus.Pending));
        dbContext.PlanetConstructionOrders.Add(CreateConstructionOrder(otherPlanetId, 1, nowUtc.AddMinutes(-1), ConstructionQueueItemStatus.Active));
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
        Assert.Equal(new QueueMaterializationSummary(0, 1, 1), result.Construction);
        Assert.Equal(new QueueMaterializationSummary(0, 1, 1), result.Research);
        Assert.Equal(new QueueMaterializationSummary(0, 1, 1), result.Shipyard);
        Assert.All(dbContext.PlanetConstructionOrders, x => Assert.NotEqual(ConstructionQueueItemStatus.Completed, x.Status));
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
