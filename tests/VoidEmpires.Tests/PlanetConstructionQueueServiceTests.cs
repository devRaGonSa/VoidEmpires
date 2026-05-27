using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.Buildings;
using VoidEmpires.Domain.Buildings;
using VoidEmpires.Domain.Economy;
using VoidEmpires.Infrastructure.Buildings;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Tests;

public class PlanetConstructionQueueServiceTests
{
    [Fact]
    public async Task EnqueueAsyncCreatesConstructionOrderAndSpendsResources()
    {
        await using var db = CreateDb();
        var planetId = Guid.NewGuid();
        var civilizationId = Guid.NewGuid();
        var requestedAtUtc = new DateTime(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);

        db.PlanetBuildingCapacities.Add(PlanetBuildingCapacity.Create(planetId, 100));
        var stockpile = PlanetResourceStockpile.Create(planetId);
        stockpile.Increase(ResourceType.Metal, 100);
        stockpile.Increase(ResourceType.Crystal, 100);
        db.PlanetResourceStockpiles.Add(stockpile);
        await db.SaveChangesAsync();

        var service = new PlanetConstructionQueueService(db);

        var result = await service.EnqueueAsync(new EnqueueConstructionOrderRequest(
            planetId,
            civilizationId,
            ConstructionQueueItemAction.Construct,
            BuildingType.MetalMine,
            requestedAtUtc));

        Assert.True(result.Succeeded);
        Assert.NotNull(result.OrderId);
        Assert.Equal(requestedAtUtc, result.StartsAtUtc);
        Assert.Equal(requestedAtUtc.AddMinutes(5), result.EndsAtUtc);
        Assert.Single(db.PlanetConstructionOrders);
        Assert.Empty(db.PlanetBuildings);
        Assert.Equal(40, stockpile.Metal);
        Assert.Equal(85, stockpile.Crystal);
    }

    [Fact]
    public async Task EnqueueAsyncRejectsSecondOpenOrderForSamePlanet()
    {
        await using var db = CreateDb();
        var planetId = Guid.NewGuid();
        var civilizationId = Guid.NewGuid();
        var requestedAtUtc = new DateTime(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);

        db.PlanetBuildingCapacities.Add(PlanetBuildingCapacity.Create(planetId, 100));
        db.PlanetConstructionOrders.Add(PlanetConstructionOrder.Create(
            planetId,
            ConstructionQueueItemAction.Construct,
            BuildingType.CommandCenter,
            1,
            1,
            requestedAtUtc,
            requestedAtUtc.AddMinutes(5),
            ConstructionQueueItemStatus.Active));
        db.PlanetResourceStockpiles.Add(PlanetResourceStockpile.Create(planetId));
        await db.SaveChangesAsync();

        var service = new PlanetConstructionQueueService(db);

        var result = await service.EnqueueAsync(new EnqueueConstructionOrderRequest(
            planetId,
            civilizationId,
            ConstructionQueueItemAction.Construct,
            BuildingType.MetalMine,
            requestedAtUtc));

        Assert.False(result.Succeeded);
        Assert.Equal(["Planet already has an open construction order."], result.Errors);
    }

    [Fact]
    public async Task EnqueueAsyncCreatesUpgradeOrderWithoutChangingBuildingLevel()
    {
        await using var db = CreateDb();
        var planetId = Guid.NewGuid();
        var civilizationId = Guid.NewGuid();
        var requestedAtUtc = new DateTime(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);
        var building = PlanetBuilding.Create(planetId, BuildingType.MetalMine, 1, 5);
        var stockpile = PlanetResourceStockpile.Create(planetId);
        stockpile.Increase(ResourceType.Metal, 500);
        stockpile.Increase(ResourceType.Crystal, 500);

        db.PlanetBuildings.Add(building);
        db.PlanetResourceStockpiles.Add(stockpile);
        await db.SaveChangesAsync();

        var service = new PlanetConstructionQueueService(db);

        var result = await service.EnqueueAsync(new EnqueueConstructionOrderRequest(
            planetId,
            civilizationId,
            ConstructionQueueItemAction.Upgrade,
            BuildingType.MetalMine,
            requestedAtUtc));

        Assert.True(result.Succeeded);
        Assert.Single(db.PlanetConstructionOrders);
        Assert.Equal(1, building.Level);
        Assert.Equal(380, stockpile.Metal);
        Assert.Equal(470, stockpile.Crystal);
    }

    private static VoidEmpiresDbContext CreateDb()
    {
        return new VoidEmpiresDbContext(
            new DbContextOptionsBuilder<VoidEmpiresDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);
    }
}
