using Microsoft.EntityFrameworkCore;
using VoidEmpires.Domain.Buildings;
using VoidEmpires.Infrastructure.Buildings;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Tests;

public class ConstructionOrderCompletionServiceTests
{
    [Fact]
    public async Task CompleteDueOrdersAsyncCreatesBuildingForDueConstructionOrder()
    {
        await using var db = CreateDb();
        var planetId = Guid.NewGuid();
        var startsAtUtc = new DateTime(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);
        var endsAtUtc = startsAtUtc.AddMinutes(5);
        var order = PlanetConstructionOrder.Create(
            planetId,
            ConstructionQueueItemAction.Construct,
            BuildingType.MetalMine,
            1,
            1,
            startsAtUtc,
            endsAtUtc,
            ConstructionQueueItemStatus.Active);

        db.PlanetConstructionOrders.Add(order);
        await db.SaveChangesAsync();

        var service = new ConstructionOrderCompletionService(db);

        var result = await service.CompleteDueOrdersAsync(endsAtUtc);

        Assert.Equal(1, result.CompletedCount);
        Assert.Equal([order.Id], result.CompletedOrderIds);
        Assert.Equal(ConstructionQueueItemStatus.Completed, order.Status);
        var building = Assert.Single(db.PlanetBuildings);
        Assert.Equal(planetId, building.PlanetId);
        Assert.Equal(BuildingType.MetalMine, building.BuildingType);
        Assert.Equal(1, building.Level);
    }

    [Fact]
    public async Task CompleteDueOrdersAsyncUpgradesBuildingForDueUpgradeOrder()
    {
        await using var db = CreateDb();
        var planetId = Guid.NewGuid();
        var building = PlanetBuilding.Create(planetId, BuildingType.MetalMine, 1, 5);
        var startsAtUtc = new DateTime(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);
        var endsAtUtc = startsAtUtc.AddMinutes(10);
        var order = PlanetConstructionOrder.Create(
            planetId,
            ConstructionQueueItemAction.Upgrade,
            BuildingType.MetalMine,
            2,
            1,
            startsAtUtc,
            endsAtUtc,
            ConstructionQueueItemStatus.Active);

        db.PlanetBuildings.Add(building);
        db.PlanetConstructionOrders.Add(order);
        await db.SaveChangesAsync();

        var service = new ConstructionOrderCompletionService(db);

        var result = await service.CompleteDueOrdersAsync(endsAtUtc);

        Assert.Equal(1, result.CompletedCount);
        Assert.Equal(2, building.Level);
        Assert.Equal(ConstructionQueueItemStatus.Completed, order.Status);
    }

    [Fact]
    public async Task CompleteDueOrdersAsyncIgnoresOrdersThatAreNotDue()
    {
        await using var db = CreateDb();
        var planetId = Guid.NewGuid();
        var startsAtUtc = new DateTime(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);
        var endsAtUtc = startsAtUtc.AddMinutes(5);
        var order = PlanetConstructionOrder.Create(
            planetId,
            ConstructionQueueItemAction.Construct,
            BuildingType.MetalMine,
            1,
            1,
            startsAtUtc,
            endsAtUtc,
            ConstructionQueueItemStatus.Active);

        db.PlanetConstructionOrders.Add(order);
        await db.SaveChangesAsync();

        var service = new ConstructionOrderCompletionService(db);

        var result = await service.CompleteDueOrdersAsync(endsAtUtc.AddTicks(-1));

        Assert.Equal(0, result.CompletedCount);
        Assert.Empty(result.CompletedOrderIds);
        Assert.Equal(ConstructionQueueItemStatus.Active, order.Status);
        Assert.Empty(db.PlanetBuildings);
    }

    private static VoidEmpiresDbContext CreateDb()
    {
        return new VoidEmpiresDbContext(
            new DbContextOptionsBuilder<VoidEmpiresDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);
    }
}
