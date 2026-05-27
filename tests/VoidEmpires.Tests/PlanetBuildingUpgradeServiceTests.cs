using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.Buildings;
using VoidEmpires.Domain.Buildings;
using VoidEmpires.Domain.Economy;
using VoidEmpires.Infrastructure.Buildings;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Tests;

public class PlanetBuildingUpgradeServiceTests
{
    [Fact]
    public async Task UpgradeAsyncIncreasesLevelAndSpendsResources()
    {
        await using var db = CreateDb();
        var planetId = Guid.NewGuid();
        var building = PlanetBuilding.Create(planetId, BuildingType.MetalMine, 1, 5);
        var stockpile = PlanetResourceStockpile.Create(planetId);
        stockpile.Increase(ResourceType.Metal, 500);
        stockpile.Increase(ResourceType.Crystal, 500);

        db.PlanetBuildings.Add(building);
        db.PlanetResourceStockpiles.Add(stockpile);
        await db.SaveChangesAsync();

        var service = new PlanetBuildingUpgradeService(db);

        var result = await service.UpgradeAsync(new UpgradeBuildingRequest(building.Id));

        Assert.True(result.Succeeded);
        Assert.Equal(2, result.NewLevel);
        Assert.Equal(2, building.Level);
        Assert.Equal(380, stockpile.Metal);
        Assert.Equal(470, stockpile.Crystal);
    }

    [Fact]
    public async Task UpgradeAsyncRejectsMissingBuilding()
    {
        await using var db = CreateDb();
        var service = new PlanetBuildingUpgradeService(db);

        var result = await service.UpgradeAsync(new UpgradeBuildingRequest(Guid.NewGuid()));

        Assert.False(result.Succeeded);
        Assert.Equal(["Building was not found."], result.Errors);
    }

    [Fact]
    public async Task UpgradeAsyncRejectsMissingStockpile()
    {
        await using var db = CreateDb();
        var building = PlanetBuilding.Create(Guid.NewGuid(), BuildingType.MetalMine, 1, 5);
        db.PlanetBuildings.Add(building);
        await db.SaveChangesAsync();

        var service = new PlanetBuildingUpgradeService(db);

        var result = await service.UpgradeAsync(new UpgradeBuildingRequest(building.Id));

        Assert.False(result.Succeeded);
        Assert.Equal(["Planet resource stockpile was not found."], result.Errors);
    }

    [Fact]
    public async Task UpgradeAsyncRejectsInsufficientResources()
    {
        await using var db = CreateDb();
        var planetId = Guid.NewGuid();
        var building = PlanetBuilding.Create(planetId, BuildingType.MetalMine, 1, 5);
        db.PlanetBuildings.Add(building);
        db.PlanetResourceStockpiles.Add(PlanetResourceStockpile.Create(planetId));
        await db.SaveChangesAsync();

        var service = new PlanetBuildingUpgradeService(db);

        var result = await service.UpgradeAsync(new UpgradeBuildingRequest(building.Id));

        Assert.False(result.Succeeded);
        Assert.Equal(["Insufficient resources."], result.Errors);
        Assert.Equal(1, building.Level);
    }

    private static VoidEmpiresDbContext CreateDb()
    {
        return new VoidEmpiresDbContext(
            new DbContextOptionsBuilder<VoidEmpiresDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);
    }
}
