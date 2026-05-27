using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.Buildings;
using VoidEmpires.Domain.Buildings;
using VoidEmpires.Domain.Economy;
using VoidEmpires.Infrastructure.Buildings;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Tests;

public class PlanetBuildingConstructionServiceTests
{
    [Fact]
    public async Task ConstructAsyncCreatesBuildingWhenCapacityAndResourcesExist()
    {
        await using var db = CreateDb();
        var planetId = Guid.NewGuid();
        var civilizationId = Guid.NewGuid();

        db.PlanetBuildingCapacities.Add(PlanetBuildingCapacity.Create(planetId, 100));
        var stockpile = PlanetResourceStockpile.Create(planetId);
        stockpile.Increase(ResourceType.Metal, 100);
        stockpile.Increase(ResourceType.Crystal, 100);
        db.PlanetResourceStockpiles.Add(stockpile);
        await db.SaveChangesAsync();

        var service = new PlanetBuildingConstructionService(db);

        var result = await service.ConstructAsync(
            new ConstructBuildingRequest(planetId, civilizationId, BuildingType.MetalMine));

        Assert.True(result.Succeeded);
        Assert.Single(db.PlanetBuildings);
        Assert.Equal(40, stockpile.Metal);
        Assert.Equal(85, stockpile.Crystal);
    }

    [Fact]
    public async Task ConstructAsyncRejectsWhenCapacityExceeded()
    {
        await using var db = CreateDb();
        var planetId = Guid.NewGuid();
        var civilizationId = Guid.NewGuid();

        db.PlanetBuildingCapacities.Add(PlanetBuildingCapacity.Create(planetId, 10));
        db.PlanetBuildings.Add(PlanetBuilding.Create(planetId, BuildingType.CommandCenter, 1, 10));
        var stockpile = PlanetResourceStockpile.Create(planetId);
        stockpile.Increase(ResourceType.Metal, 100);
        stockpile.Increase(ResourceType.Crystal, 100);
        db.PlanetResourceStockpiles.Add(stockpile);
        await db.SaveChangesAsync();

        var service = new PlanetBuildingConstructionService(db);

        var result = await service.ConstructAsync(
            new ConstructBuildingRequest(planetId, civilizationId, BuildingType.MetalMine));

        Assert.False(result.Succeeded);
        Assert.Equal(["Planet building capacity would be exceeded."], result.Errors);
    }

    [Fact]
    public async Task ConstructAsyncRejectsWhenResourcesAreInsufficient()
    {
        await using var db = CreateDb();
        var planetId = Guid.NewGuid();
        var civilizationId = Guid.NewGuid();

        db.PlanetBuildingCapacities.Add(PlanetBuildingCapacity.Create(planetId, 100));
        db.PlanetResourceStockpiles.Add(PlanetResourceStockpile.Create(planetId));
        await db.SaveChangesAsync();

        var service = new PlanetBuildingConstructionService(db);

        var result = await service.ConstructAsync(
            new ConstructBuildingRequest(planetId, civilizationId, BuildingType.MetalMine));

        Assert.False(result.Succeeded);
        Assert.Equal(["Insufficient resources."], result.Errors);
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
