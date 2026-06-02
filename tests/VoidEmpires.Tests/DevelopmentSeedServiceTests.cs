using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.Development;
using VoidEmpires.Application.StrategicMap;
using VoidEmpires.Domain.Colonization;
using VoidEmpires.Domain.Assets;
using VoidEmpires.Domain.Economy;
using VoidEmpires.Domain.Fleets;
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
    private static readonly Guid IndustrialPlanetId = Guid.Parse("40000000-0000-0000-0000-000000000002");
    private static readonly Guid OrbitalPlanetId = Guid.Parse("40000000-0000-0000-0000-000000000003");

    [Fact]
    public async Task ApplyAsyncCreatesExpectedStrategicMapDataset()
    {
        await using var dbContext = CreateDbContext();

        var result = await new DevelopmentSeedService(dbContext)
            .ApplyAsync(new ApplyDevelopmentSeedRequest("minimal-validation"));

        Assert.True(result.Succeeded);
        Assert.Contains(result.AppliedSteps, x => x.Contains(CivilizationId.ToString(), StringComparison.Ordinal));
        Assert.True(await dbContext.Set<SolarSystem>().AnyAsync(x => x.Id == SystemId));
        Assert.True(await dbContext.Set<Planet>().CountAsync(x => x.SolarSystemId == SystemId) >= 3);
        Assert.True(await dbContext.Set<Planet>().AnyAsync(x => x.Id == OwnedPlanetId && x.PlanetType == PlanetType.Terran));
        Assert.True(await dbContext.Set<Planet>().AnyAsync(x => x.Id == IndustrialPlanetId && x.PlanetType == PlanetType.Desert));
        Assert.True(await dbContext.Set<Planet>().AnyAsync(x => x.Id == OrbitalPlanetId && x.PlanetType == PlanetType.GasGiant));
        Assert.True(await dbContext.Set<PlanetOwnership>().AnyAsync(x => x.PlanetId == OwnedPlanetId && x.CivilizationId == CivilizationId));
        var stockpile = await dbContext.PlanetResourceStockpiles.SingleAsync(x => x.PlanetId == OwnedPlanetId);
        Assert.Equal(125, stockpile.Credits);
        Assert.Equal(80, stockpile.Metal);
        Assert.Equal(35, stockpile.Crystal);
        Assert.Equal(20, stockpile.Gas);
        Assert.True(await dbContext.Set<OrbitalAssetStock>().AnyAsync(x => x.PlanetId == OwnedPlanetId && x.AssetType == SpaceAssetType.EscortCraft && x.Quantity == 4));
        Assert.Equal(4, await dbContext.Set<OrbitalGroup>().CountAsync(x => x.CivilizationId == CivilizationId));
        Assert.True(await dbContext.Set<OrbitalGroup>().AnyAsync(x =>
            x.CivilizationId == CivilizationId &&
            x.OriginPlanetId == OwnedPlanetId &&
            x.CurrentPlanetId == OwnedPlanetId &&
            x.AssetType == SpaceAssetType.ScoutCraft &&
            x.Quantity == 3 &&
            x.Status == OrbitalGroupStatus.Stationed));
        Assert.True(await dbContext.Set<OrbitalGroup>().AnyAsync(x =>
            x.CivilizationId == CivilizationId &&
            x.OriginPlanetId == OwnedPlanetId &&
            x.CurrentPlanetId == OwnedPlanetId &&
            x.AssetType == SpaceAssetType.ScoutCraft &&
            x.Quantity == 2 &&
            x.Status == OrbitalGroupStatus.Stationed));
        Assert.True(await dbContext.Set<OrbitalGroup>().AnyAsync(x =>
            x.CivilizationId == CivilizationId &&
            x.OriginPlanetId == OwnedPlanetId &&
            x.CurrentPlanetId == OrbitalPlanetId &&
            x.AssetType == SpaceAssetType.EscortCraft &&
            x.Quantity == 4 &&
            x.Status == OrbitalGroupStatus.Stationed));
        Assert.True(await dbContext.Set<OrbitalGroup>().AnyAsync(x =>
            x.CivilizationId == CivilizationId &&
            x.OriginPlanetId == OwnedPlanetId &&
            x.CurrentPlanetId == OwnedPlanetId &&
            x.AssetType == SpaceAssetType.CargoCraft &&
            x.Quantity == 2 &&
            x.Status == OrbitalGroupStatus.Reserved));
        Assert.True(await dbContext.Set<OrbitalTransfer>().AnyAsync(x => x.CivilizationId == CivilizationId && x.DestinationPlanetId != x.OriginPlanetId && x.Status == OrbitalTransferStatus.Planned));
    }

    [Fact]
    public async Task ApplyAsyncIsIdempotent()
    {
        await using var dbContext = CreateDbContext();
        var service = new DevelopmentSeedService(dbContext);

        _ = await service.ApplyAsync(new ApplyDevelopmentSeedRequest("minimal-validation"));
        var planetCountAfterFirstRun = await dbContext.Set<Planet>().CountAsync(x => x.SolarSystemId == SystemId);
        _ = await service.ApplyAsync(new ApplyDevelopmentSeedRequest("minimal-validation"));

        Assert.Equal(1, await dbContext.Set<SolarSystem>().CountAsync(x => x.Id == SystemId));
        Assert.Equal(planetCountAfterFirstRun, await dbContext.Set<Planet>().CountAsync(x => x.SolarSystemId == SystemId));
        Assert.True(planetCountAfterFirstRun >= 3);
        Assert.Equal(1, await dbContext.Set<Planet>().CountAsync(x => x.Id == OwnedPlanetId));
        Assert.Equal(1, await dbContext.Set<Planet>().CountAsync(x => x.Id == IndustrialPlanetId));
        Assert.Equal(1, await dbContext.Set<Planet>().CountAsync(x => x.Id == OrbitalPlanetId));
        Assert.Equal(1, await dbContext.Set<PlanetOwnership>().CountAsync(x => x.PlanetId == OwnedPlanetId && x.CivilizationId == CivilizationId));
        Assert.Equal(1, await dbContext.PlanetResourceStockpiles.CountAsync(x => x.PlanetId == OwnedPlanetId));
        Assert.Equal(1, await dbContext.Set<OrbitalAssetStock>().CountAsync(x => x.PlanetId == OwnedPlanetId && x.AssetType == SpaceAssetType.EscortCraft));
        Assert.Equal(4, await dbContext.Set<OrbitalGroup>().CountAsync(x => x.CivilizationId == CivilizationId));
        Assert.Equal(1, await dbContext.Set<OrbitalGroup>().CountAsync(x =>
            x.CivilizationId == CivilizationId &&
            x.OriginPlanetId == OwnedPlanetId &&
            x.CurrentPlanetId == OwnedPlanetId &&
            x.AssetType == SpaceAssetType.ScoutCraft &&
            x.Quantity == 3 &&
            x.Status == OrbitalGroupStatus.Stationed));
        Assert.Equal(1, await dbContext.Set<OrbitalGroup>().CountAsync(x =>
            x.CivilizationId == CivilizationId &&
            x.OriginPlanetId == OwnedPlanetId &&
            x.CurrentPlanetId == OwnedPlanetId &&
            x.AssetType == SpaceAssetType.ScoutCraft &&
            x.Quantity == 2 &&
            x.Status == OrbitalGroupStatus.Stationed));
        Assert.Equal(1, await dbContext.Set<OrbitalGroup>().CountAsync(x =>
            x.CivilizationId == CivilizationId &&
            x.OriginPlanetId == OwnedPlanetId &&
            x.CurrentPlanetId == OrbitalPlanetId &&
            x.AssetType == SpaceAssetType.EscortCraft &&
            x.Quantity == 4 &&
            x.Status == OrbitalGroupStatus.Stationed));
        Assert.Equal(1, await dbContext.Set<OrbitalGroup>().CountAsync(x =>
            x.CivilizationId == CivilizationId &&
            x.OriginPlanetId == OwnedPlanetId &&
            x.CurrentPlanetId == OwnedPlanetId &&
            x.AssetType == SpaceAssetType.CargoCraft &&
            x.Quantity == 2 &&
            x.Status == OrbitalGroupStatus.Reserved));
        Assert.Equal(1, await dbContext.Set<OrbitalTransfer>().CountAsync(x => x.CivilizationId == CivilizationId && x.Status == OrbitalTransferStatus.Planned));
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
        Assert.True(system.Planets.Count >= 3);
        Assert.Contains(system.Planets, x => x.PlanetId == OwnedPlanetId && x.IsOwnedByRequestingCivilization);
        Assert.Contains(system.Planets, x => x.PlanetId == IndustrialPlanetId);
        Assert.Contains(system.Planets, x => x.PlanetId == OrbitalPlanetId);
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
