using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.Development;
using VoidEmpires.Application.StrategicMap;
using VoidEmpires.Domain.Colonization;
using VoidEmpires.Domain.Assets;
using VoidEmpires.Domain.Economy;
using VoidEmpires.Domain.Fleets;
using VoidEmpires.Domain.Galaxy;
using VoidEmpires.Domain.Players;
using VoidEmpires.Infrastructure.Development;
using VoidEmpires.Infrastructure.Persistence;
using VoidEmpires.Infrastructure.StrategicMap;
using VoidEmpires.Infrastructure.Visuals;

namespace VoidEmpires.Tests;

public class DevelopmentSeedServiceTests
{
    private static readonly Guid PlayerProfileId = Guid.Parse("90000000-0000-0000-0000-000000000001");
    private static readonly Guid CivilizationId = Guid.Parse("00000000-0000-0000-0000-000000000001");
    private static readonly Guid GalaxyId = Guid.Parse("10000000-0000-0000-0000-000000000001");
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
        var playerProfile = await dbContext.Set<PlayerProfile>().SingleAsync(x => x.Id == PlayerProfileId);
        var civilization = await dbContext.Set<Civilization>().SingleAsync(x => x.Id == CivilizationId);
        Assert.Equal(PlayerProfileId, playerProfile.Id);
        Assert.Equal(CivilizationId, civilization.Id);
        Assert.Equal(PlayerProfileId, civilization.PlayerProfileId);
        Assert.Equal(OwnedPlanetId, civilization.HomePlanetId);
        var galaxy = await dbContext.Galaxies.SingleAsync(x => x.Id == GalaxyId);
        var solarSystem = await dbContext.Set<SolarSystem>().SingleAsync(x => x.Id == SystemId);
        Assert.Equal(GalaxyId, galaxy.Id);
        Assert.Equal(GalaxyId, solarSystem.GalaxyId);
        Assert.Equal(3, await dbContext.Set<Planet>().CountAsync(x => x.SolarSystemId == SystemId));
        Assert.True(await dbContext.Set<PlanetOwnership>().AnyAsync(x => x.PlanetId == OwnedPlanetId && x.CivilizationId == CivilizationId));
        var stockpile = await dbContext.PlanetResourceStockpiles.SingleAsync(x => x.PlanetId == OwnedPlanetId);
        Assert.Equal(125, stockpile.Credits);
        Assert.Equal(80, stockpile.Metal);
        Assert.Equal(35, stockpile.Crystal);
        Assert.Equal(20, stockpile.Gas);
        var productionProfile = await dbContext.PlanetProductionProfiles.SingleAsync(x => x.PlanetId == OwnedPlanetId);
        Assert.Equal(18, productionProfile.CreditsPerHour);
        Assert.Equal(14, productionProfile.MetalPerHour);
        Assert.Equal(6, productionProfile.CrystalPerHour);
        Assert.Equal(3, productionProfile.GasPerHour);
        Assert.True(await dbContext.Set<OrbitalAssetStock>().AnyAsync(x => x.PlanetId == OwnedPlanetId && x.AssetType == SpaceAssetType.EscortCraft && x.Quantity == 4));
        Assert.Equal(4, await dbContext.Set<OrbitalGroup>().CountAsync(x => x.CivilizationId == CivilizationId));
        Assert.True(await dbContext.Set<OrbitalGroup>().AnyAsync(
            x => x.CivilizationId == CivilizationId &&
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
        _ = await service.ApplyAsync(new ApplyDevelopmentSeedRequest("minimal-validation"));

        Assert.Equal(1, await dbContext.Set<PlayerProfile>().CountAsync(x => x.Id == PlayerProfileId));
        Assert.Equal(1, await dbContext.Set<Civilization>().CountAsync(x => x.Id == CivilizationId));
        Assert.Equal(1, await dbContext.Galaxies.CountAsync(x => x.Id == GalaxyId));
        Assert.Equal(1, await dbContext.Set<SolarSystem>().CountAsync(x => x.Id == SystemId));
        Assert.Equal(3, await dbContext.Set<Planet>().CountAsync(x => x.SolarSystemId == SystemId));
        Assert.Equal(1, await dbContext.Set<PlanetOwnership>().CountAsync(x => x.PlanetId == OwnedPlanetId && x.CivilizationId == CivilizationId));
        Assert.Equal(1, await dbContext.PlanetResourceStockpiles.CountAsync(x => x.PlanetId == OwnedPlanetId));
        Assert.Equal(1, await dbContext.PlanetProductionProfiles.CountAsync(x => x.PlanetId == OwnedPlanetId));
        Assert.Equal(1, await dbContext.Set<OrbitalAssetStock>().CountAsync(x => x.PlanetId == OwnedPlanetId && x.AssetType == SpaceAssetType.EscortCraft));
        Assert.Equal(4, await dbContext.Set<OrbitalGroup>().CountAsync(x => x.CivilizationId == CivilizationId));
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
