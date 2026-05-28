using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.Fleets;
using VoidEmpires.Domain.Assets;
using VoidEmpires.Domain.Fleets;
using VoidEmpires.Infrastructure.Fleets;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Tests;

public class OrbitalStockGroupServiceTests
{
    [Fact]
    public async Task CreateFromLocalStockCreatesOrbitalGroupAndConsumesStock()
    {
        await using var dbContext = CreateDbContext();
        var originPlanetId = Guid.NewGuid();
        var currentPlanetId = Guid.NewGuid();
        var civilizationId = Guid.NewGuid();
        var stock = OrbitalAssetStock.Create(originPlanetId, SpaceAssetType.ScoutCraft, 5);
        dbContext.Set<OrbitalAssetStock>().Add(stock);
        await dbContext.SaveChangesAsync();

        var service = new OrbitalStockGroupService(dbContext);

        var result = await service.CreateFromLocalStockAsync(new CreateOrbitalGroupRequest(
            civilizationId,
            originPlanetId,
            currentPlanetId,
            SpaceAssetType.ScoutCraft,
            2));

        Assert.True(result.Succeeded);
        Assert.NotNull(result.OrbitalGroupId);
        Assert.Equal(3, stock.Quantity);

        var group = await dbContext.Set<OrbitalGroup>().SingleAsync();
        Assert.Equal(civilizationId, group.CivilizationId);
        Assert.Equal(originPlanetId, group.OriginPlanetId);
        Assert.Equal(currentPlanetId, group.CurrentPlanetId);
        Assert.Equal(SpaceAssetType.ScoutCraft, group.AssetType);
        Assert.Equal(2, group.Quantity);
        Assert.True(group.IsStationedAwayFromOrigin);
    }

    [Fact]
    public async Task CreateFromLocalStockRejectsInsufficientStock()
    {
        await using var dbContext = CreateDbContext();
        var originPlanetId = Guid.NewGuid();
        var stock = OrbitalAssetStock.Create(originPlanetId, SpaceAssetType.CargoCraft, 1);
        dbContext.Set<OrbitalAssetStock>().Add(stock);
        await dbContext.SaveChangesAsync();

        var service = new OrbitalStockGroupService(dbContext);

        var result = await service.CreateFromLocalStockAsync(new CreateOrbitalGroupRequest(
            Guid.NewGuid(),
            originPlanetId,
            Guid.NewGuid(),
            SpaceAssetType.CargoCraft,
            2));

        Assert.False(result.Succeeded);
        Assert.Null(result.OrbitalGroupId);
        Assert.Equal(1, stock.Quantity);
        Assert.Empty(await dbContext.Set<OrbitalGroup>().ToListAsync());
    }

    [Fact]
    public async Task CreateFromLocalStockRejectsInvalidRequest()
    {
        await using var dbContext = CreateDbContext();
        var service = new OrbitalStockGroupService(dbContext);

        var result = await service.CreateFromLocalStockAsync(new CreateOrbitalGroupRequest(
            Guid.Empty,
            Guid.NewGuid(),
            Guid.NewGuid(),
            SpaceAssetType.ScoutCraft,
            1));

        Assert.False(result.Succeeded);
        Assert.Null(result.OrbitalGroupId);
    }

    private static VoidEmpiresDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<VoidEmpiresDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new VoidEmpiresDbContext(options);
    }
}
