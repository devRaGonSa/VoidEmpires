using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.Buildings;
using VoidEmpires.Domain.Buildings;
using VoidEmpires.Infrastructure.Buildings;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Tests;

public class PlanetBuildingConstructionServiceTests
{
    [Fact]
    public async Task ConstructAsyncCreatesBuildingWhenCapacityExists()
    {
        await using var db = CreateDb();
        var planetId = Guid.NewGuid();

        db.PlanetBuildingCapacities.Add(PlanetBuildingCapacity.Create(planetId, 100));
        await db.SaveChangesAsync();

        var service = new PlanetBuildingConstructionService(db);

        var result = await service.ConstructAsync(
            new ConstructBuildingRequest(planetId, BuildingType.MetalMine, 1, 10));

        Assert.True(result.Succeeded);
        Assert.Single(db.PlanetBuildings);
    }

    [Fact]
    public async Task ConstructAsyncRejectsWhenCapacityExceeded()
    {
        await using var db = CreateDb();
        var planetId = Guid.NewGuid();

        db.PlanetBuildingCapacities.Add(PlanetBuildingCapacity.Create(planetId, 10));
        db.PlanetBuildings.Add(PlanetBuilding.Create(planetId, BuildingType.CommandCenter, 1, 10));
        await db.SaveChangesAsync();

        var service = new PlanetBuildingConstructionService(db);

        var result = await service.ConstructAsync(
            new ConstructBuildingRequest(planetId, BuildingType.MetalMine, 1, 1));

        Assert.False(result.Succeeded);
    }

    private static VoidEmpiresDbContext CreateDb()
    {
        return new VoidEmpiresDbContext(
            new DbContextOptionsBuilder<VoidEmpiresDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);
    }
}
