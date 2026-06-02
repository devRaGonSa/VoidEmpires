using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.Development;
using VoidEmpires.Application.StrategicMap;
using VoidEmpires.Domain.Colonization;
using VoidEmpires.Domain.Economy;
using VoidEmpires.Domain.Galaxy;
using VoidEmpires.Infrastructure.Development;
using VoidEmpires.Infrastructure.Persistence;
using VoidEmpires.Infrastructure.StrategicMap;
using VoidEmpires.Infrastructure.Visuals;

namespace VoidEmpires.Tests;

public class DevelopmentSeedServiceTests
{
    private static readonly Guid CivilizationId = Guid.Parse("00000000-0000-0000-0000-000000000001");
    private static readonly Guid SystemId = Guid.Parse("20000000-0000-0000-0000-000000000001");
    private static readonly Guid OwnedPlanetId = Guid.Parse("40000000-0000-0000-0000-000000000001");

    [Fact]
    public async Task ApplyAsyncCreatesExpectedStrategicMapDataset()
    {
        await using var dbContext = CreateDbContext();

        var result = await new DevelopmentSeedService(dbContext)
            .ApplyAsync(new ApplyDevelopmentSeedRequest("minimal-validation"));

        Assert.True(result.Succeeded);
        Assert.Contains(result.AppliedSteps, x => x.Contains(CivilizationId.ToString(), StringComparison.Ordinal));
        Assert.True(await dbContext.Set<SolarSystem>().AnyAsync(x => x.Id == SystemId));
        Assert.Equal(3, await dbContext.Set<Planet>().CountAsync(x => x.SolarSystemId == SystemId));
        Assert.True(await dbContext.Set<PlanetOwnership>().AnyAsync(x => x.PlanetId == OwnedPlanetId && x.CivilizationId == CivilizationId));
        var stockpile = await dbContext.PlanetResourceStockpiles.SingleAsync(x => x.PlanetId == OwnedPlanetId);
        Assert.Equal(125, stockpile.Credits);
        Assert.Equal(80, stockpile.Metal);
        Assert.Equal(35, stockpile.Crystal);
        Assert.Equal(20, stockpile.Gas);
    }

    [Fact]
    public async Task ApplyAsyncIsIdempotent()
    {
        await using var dbContext = CreateDbContext();
        var service = new DevelopmentSeedService(dbContext);

        _ = await service.ApplyAsync(new ApplyDevelopmentSeedRequest("minimal-validation"));
        _ = await service.ApplyAsync(new ApplyDevelopmentSeedRequest("minimal-validation"));

        Assert.Equal(1, await dbContext.Set<SolarSystem>().CountAsync(x => x.Id == SystemId));
        Assert.Equal(3, await dbContext.Set<Planet>().CountAsync(x => x.SolarSystemId == SystemId));
        Assert.Equal(1, await dbContext.Set<PlanetOwnership>().CountAsync(x => x.PlanetId == OwnedPlanetId && x.CivilizationId == CivilizationId));
        Assert.Equal(1, await dbContext.PlanetResourceStockpiles.CountAsync(x => x.PlanetId == OwnedPlanetId));
    }

    [Fact]
    public async Task StrategicMapServiceReturnsNonEmptySystemsFromSeededDataset()
    {
        await using var dbContext = CreateDbContext();
        await new DevelopmentSeedService(dbContext).ApplyAsync(new ApplyDevelopmentSeedRequest("minimal-validation"));

        var result = await CreateStrategicMapService(dbContext).GetAsync(new GetStrategicMapRequest(CivilizationId));

        var system = Assert.Single(result.Systems);
        Assert.Equal(SystemId, system.SystemId);
        Assert.Equal(MapVisibilityLevel.Visible, system.VisibilityLevel);
        Assert.True(system.IsVisible);
        Assert.Equal(3, system.Planets.Count);
        Assert.Contains(system.Planets, x => x.PlanetId == OwnedPlanetId && x.IsOwnedByRequestingCivilization);
    }

    private static StrategicMapService CreateStrategicMapService(VoidEmpiresDbContext dbContext) =>
        new(
            dbContext,
            new SystemVisualStateService(dbContext, new PlanetVisualStateService(dbContext)),
            new MapVisibilityService(dbContext));

    private static VoidEmpiresDbContext CreateDbContext() =>
        new(new DbContextOptionsBuilder<VoidEmpiresDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);
}
