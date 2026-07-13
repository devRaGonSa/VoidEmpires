using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.Fleets;
using VoidEmpires.Domain.Assets;
using VoidEmpires.Domain.Fleets;
using VoidEmpires.Domain.Colonization;
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
        var civilizationId = Guid.NewGuid();
        var stock = OrbitalAssetStock.Create(originPlanetId, SpaceAssetType.ScoutCraft, 5);
        dbContext.Set<OrbitalAssetStock>().Add(stock);
        dbContext.PlanetOwnerships.Add(PlanetOwnership.Create(originPlanetId, civilizationId));
        await dbContext.SaveChangesAsync();

        var service = new OrbitalStockGroupService(dbContext);

        var result = await service.CreateFromLocalStockAsync(new CreateOrbitalGroupRequest(
            civilizationId,
            originPlanetId,
            originPlanetId,
            SpaceAssetType.ScoutCraft,
            2));

        Assert.True(result.Succeeded);
        Assert.NotNull(result.OrbitalGroupId);
        Assert.Equal(3, stock.Quantity);

        var group = await dbContext.Set<OrbitalGroup>().SingleAsync();
        Assert.Equal(civilizationId, group.CivilizationId);
        Assert.Equal(originPlanetId, group.OriginPlanetId);
        Assert.Equal(originPlanetId, group.CurrentPlanetId);
        Assert.Equal(SpaceAssetType.ScoutCraft, group.AssetType);
        Assert.Equal(2, group.Quantity);
        Assert.False(group.IsStationedAwayFromOrigin);
    }

    [Fact]
    public async Task CreateFromLocalStockRejectsInsufficientStock()
    {
        await using var dbContext = CreateDbContext();
        var originPlanetId = Guid.NewGuid();
        var stock = OrbitalAssetStock.Create(originPlanetId, SpaceAssetType.CargoCraft, 1);
        var civilizationId = Guid.NewGuid();
        dbContext.Set<OrbitalAssetStock>().Add(stock);
        dbContext.PlanetOwnerships.Add(PlanetOwnership.Create(originPlanetId, civilizationId));
        await dbContext.SaveChangesAsync();

        var service = new OrbitalStockGroupService(dbContext);

        var result = await service.CreateFromLocalStockAsync(new CreateOrbitalGroupRequest(
            civilizationId,
            originPlanetId,
            originPlanetId,
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

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task CreateFromLocalStockRejectsNonPositiveQuantity(int quantity)
    {
        await using var dbContext = CreateDbContext();
        var civilizationId = Guid.NewGuid();
        var planetId = Guid.NewGuid();
        dbContext.PlanetOwnerships.Add(PlanetOwnership.Create(planetId, civilizationId));
        dbContext.Set<OrbitalAssetStock>().Add(OrbitalAssetStock.Create(planetId, SpaceAssetType.ScoutCraft, 2));
        await dbContext.SaveChangesAsync();

        var result = await new OrbitalStockGroupService(dbContext).CreateFromLocalStockAsync(
            new CreateOrbitalGroupRequest(civilizationId, planetId, planetId, SpaceAssetType.ScoutCraft, quantity));

        Assert.False(result.Succeeded);
        Assert.Empty(await dbContext.Set<OrbitalGroup>().ToListAsync());
    }

    [Fact]
    public async Task CreateFromLocalStockRejectsAwayPlanetAndWrongOwnership()
    {
        await using var dbContext = CreateDbContext();
        var civilizationId = Guid.NewGuid();
        var originPlanetId = Guid.NewGuid();
        var awayPlanetId = Guid.NewGuid();
        var stock = OrbitalAssetStock.Create(originPlanetId, SpaceAssetType.ScoutCraft, 3);
        dbContext.Set<OrbitalAssetStock>().Add(stock);
        dbContext.PlanetOwnerships.Add(PlanetOwnership.Create(originPlanetId, civilizationId));
        await dbContext.SaveChangesAsync();

        var service = new OrbitalStockGroupService(dbContext);

        var firstResult = await service.CreateFromLocalStockAsync(new CreateOrbitalGroupRequest(
            civilizationId,
            originPlanetId,
            awayPlanetId,
            SpaceAssetType.ScoutCraft,
            1));
        var wrongOwnerResult = await service.CreateFromLocalStockAsync(new CreateOrbitalGroupRequest(
            Guid.NewGuid(),
            originPlanetId,
            originPlanetId,
            SpaceAssetType.ScoutCraft,
            1));

        Assert.False(firstResult.Succeeded);
        Assert.False(wrongOwnerResult.Succeeded);
        Assert.Equal(3, stock.Quantity);
        Assert.Empty(await dbContext.Set<OrbitalGroup>().ToListAsync());
    }

    private static VoidEmpiresDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<VoidEmpiresDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new VoidEmpiresDbContext(options);
    }
}
