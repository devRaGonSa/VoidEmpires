using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.StrategicMap;
using VoidEmpires.Domain.Colonization;
using VoidEmpires.Domain.Galaxy;
using VoidEmpires.Infrastructure.Persistence;
using VoidEmpires.Infrastructure.StrategicMap;

namespace VoidEmpires.Tests;

public class MapVisibilityServiceTests
{
    [Fact]
    public async Task GetAsyncMarksOwnedPlanetAsVisibleAndOwned()
    {
        await using var dbContext = CreateDbContext();
        var civilizationId = Guid.NewGuid();
        var system = CreateSystem("Owned", 1, 2, 3);
        var planet = new Planet(Guid.NewGuid(), system.Id, "Home", 1, PlanetType.Terran, 120, PlanetColonizationStatus.Colonized);
        dbContext.Set<SolarSystem>().Add(system);
        dbContext.Set<Planet>().Add(planet);
        dbContext.Set<PlanetOwnership>().Add(PlanetOwnership.Create(planet.Id, civilizationId));
        await dbContext.SaveChangesAsync();

        var result = await CreateService(dbContext).GetAsync(new GetMapVisibilityRequest(civilizationId));

        var mapPlanet = Assert.Single(Assert.Single(result.Systems).Planets);
        Assert.Equal(MapVisibilityLevel.Owned, mapPlanet.VisibilityLevel);
        Assert.Equal(MapVisibilityReason.OwnedPlanet, mapPlanet.VisibilityReason);
        Assert.True(mapPlanet.IsVisible);
        Assert.True(mapPlanet.IsOwnedByRequestingCivilization);
        Assert.Equal(civilizationId, mapPlanet.CivilizationId);
    }

    [Fact]
    public async Task GetAsyncMarksSystemContainingOwnedPlanetAsVisible()
    {
        await using var dbContext = CreateDbContext();
        var civilizationId = Guid.NewGuid();
        var system = CreateSystem("Visible", 4, 5, 6);
        var ownedPlanet = new Planet(Guid.NewGuid(), system.Id, "Home", 1, PlanetType.Terran, 100);
        var neighborPlanet = new Planet(Guid.NewGuid(), system.Id, "Neighbor", 2, PlanetType.Ice, 80);
        dbContext.Set<SolarSystem>().Add(system);
        dbContext.Set<Planet>().AddRange(ownedPlanet, neighborPlanet);
        dbContext.Set<PlanetOwnership>().Add(PlanetOwnership.Create(ownedPlanet.Id, civilizationId));
        await dbContext.SaveChangesAsync();

        var result = await CreateService(dbContext).GetAsync(new GetMapVisibilityRequest(civilizationId));

        var mapSystem = Assert.Single(result.Systems);
        Assert.Equal(MapVisibilityLevel.Visible, mapSystem.VisibilityLevel);
        Assert.True(mapSystem.IsVisible);
        Assert.Equal("Visible", mapSystem.SystemName);
        var neighbor = mapSystem.Planets.Single(x => x.PlanetId == neighborPlanet.Id);
        Assert.Equal(MapVisibilityLevel.Visible, neighbor.VisibilityLevel);
        Assert.False(neighbor.IsOwnedByRequestingCivilization);
    }

    [Fact]
    public async Task GetAsyncDoesNotExposeOtherCivilizationOwnedPlanetAsOwned()
    {
        await using var dbContext = CreateDbContext();
        var civilizationId = Guid.NewGuid();
        var otherCivilizationId = Guid.NewGuid();
        var system = CreateSystem("Shared", 0, 0, 0);
        var ownedPlanet = new Planet(Guid.NewGuid(), system.Id, "Home", 1, PlanetType.Terran, 100);
        var otherPlanet = new Planet(Guid.NewGuid(), system.Id, "Foreign", 2, PlanetType.Desert, 90);
        dbContext.Set<SolarSystem>().Add(system);
        dbContext.Set<Planet>().AddRange(ownedPlanet, otherPlanet);
        dbContext.Set<PlanetOwnership>().AddRange(
            PlanetOwnership.Create(ownedPlanet.Id, civilizationId),
            PlanetOwnership.Create(otherPlanet.Id, otherCivilizationId));
        await dbContext.SaveChangesAsync();

        var result = await CreateService(dbContext).GetAsync(new GetMapVisibilityRequest(civilizationId));

        var otherMapPlanet = Assert.Single(result.Systems).Planets.Single(x => x.PlanetId == otherPlanet.Id);
        Assert.Equal(MapVisibilityLevel.Visible, otherMapPlanet.VisibilityLevel);
        Assert.False(otherMapPlanet.IsOwnedByRequestingCivilization);
        Assert.Null(otherMapPlanet.CivilizationId);
    }

    [Fact]
    public async Task GetAsyncRepresentsUnknownPersistedSystemsAndPlanetsWithHiddenDetails()
    {
        await using var dbContext = CreateDbContext();
        var civilizationId = Guid.NewGuid();
        var unknownSystem = CreateSystem("Uncharted", 9, 9, 9);
        var unknownPlanet = new Planet(Guid.NewGuid(), unknownSystem.Id, "Hidden", 1, PlanetType.Barren, 60);
        dbContext.Set<SolarSystem>().Add(unknownSystem);
        dbContext.Set<Planet>().Add(unknownPlanet);
        await dbContext.SaveChangesAsync();

        var result = await CreateService(dbContext).GetAsync(new GetMapVisibilityRequest(civilizationId));

        var mapSystem = Assert.Single(result.Systems);
        Assert.Equal(MapVisibilityLevel.Unknown, mapSystem.VisibilityLevel);
        Assert.Equal(MapVisibilityReason.NoKnownVisibilitySource, mapSystem.VisibilityReason);
        Assert.False(mapSystem.IsVisible);
        Assert.Null(mapSystem.SystemName);
        Assert.Null(mapSystem.CoordinateX);
        var mapPlanet = Assert.Single(mapSystem.Planets);
        Assert.Equal(MapVisibilityLevel.Unknown, mapPlanet.VisibilityLevel);
        Assert.False(mapPlanet.IsVisible);
        Assert.Null(mapPlanet.PlanetName);
        Assert.Null(mapPlanet.PlanetType);
    }

    [Fact]
    public async Task GetAsyncDoesNotMutatePersistedState()
    {
        await using var dbContext = CreateDbContext();
        var civilizationId = Guid.NewGuid();
        var system = CreateSystem("Stable", 7, 8, 9);
        var planet = new Planet(Guid.NewGuid(), system.Id, "Quiet", 1, PlanetType.Oceanic, 70);
        dbContext.Set<SolarSystem>().Add(system);
        dbContext.Set<Planet>().Add(planet);
        dbContext.Set<PlanetOwnership>().Add(PlanetOwnership.Create(planet.Id, civilizationId));
        await dbContext.SaveChangesAsync();
        var ownershipCount = await dbContext.Set<PlanetOwnership>().CountAsync();

        _ = await CreateService(dbContext).GetAsync(new GetMapVisibilityRequest(civilizationId));

        Assert.Equal(0, dbContext.ChangeTracker.Entries().Count(x => x.State != EntityState.Unchanged));
        Assert.Equal(ownershipCount, await dbContext.Set<PlanetOwnership>().CountAsync());
    }

    private static MapVisibilityService CreateService(VoidEmpiresDbContext dbContext) => new(dbContext);

    private static SolarSystem CreateSystem(string name, int x, int y, int z)
    {
        var systemId = Guid.NewGuid();
        var star = new Star(Guid.NewGuid(), systemId, $"{name} Star", StarType.YellowDwarf);
        return new SolarSystem(systemId, Guid.NewGuid(), name, new GalaxyCoordinates(x, y, z), star);
    }

    private static VoidEmpiresDbContext CreateDbContext() =>
        new(new DbContextOptionsBuilder<VoidEmpiresDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);
}
