using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.StrategicMap;
using VoidEmpires.Domain.Assets;
using VoidEmpires.Domain.Colonization;
using VoidEmpires.Domain.Exploration;
using VoidEmpires.Domain.Fleets;
using VoidEmpires.Domain.Galaxy;
using VoidEmpires.Infrastructure.Persistence;
using VoidEmpires.Infrastructure.StrategicMap;

namespace VoidEmpires.Tests;

public class SensorProfileServiceTests
{
    [Fact]
    public async Task GetAsyncReturnsEmptyResultForCivilizationWithoutSources()
    {
        await using var dbContext = CreateDbContext();
        Assert.Empty((await CreateService(dbContext).GetAsync(new GetSensorProfilesRequest(Guid.NewGuid()))).Profiles);
    }

    [Fact]
    public async Task GetAsyncReturnsDeterministicOwnedPlanetProfile()
    {
        await using var dbContext = CreateDbContext();
        var civilizationId = Guid.NewGuid();
        var system = CreateSystem("Sensor Home");
        var planet = new Planet(Guid.NewGuid(), system.Id, "Home", 1, PlanetType.Terran, 100, PlanetColonizationStatus.Colonized);
        dbContext.AddRange(system, planet, PlanetOwnership.Create(planet.Id, civilizationId));
        await dbContext.SaveChangesAsync();

        var profile = Assert.Single((await CreateService(dbContext).GetAsync(new GetSensorProfilesRequest(civilizationId))).Profiles);

        Assert.Equal(SensorProfileSourceKind.Planet, profile.SourceKind);
        Assert.Equal(SensorProfileClass.Orbital, profile.SensorClass);
        Assert.Equal((planet.Id, system.Id, 2, 20), (profile.PlanetId, profile.SolarSystemId, profile.DetectionRangeTier, profile.ScanStrength));
    }

    [Fact]
    public async Task GetAsyncReturnsScoutOrbitalGroupProfile()
    {
        await using var dbContext = CreateDbContext();
        var civilizationId = Guid.NewGuid();
        var planetId = Guid.NewGuid();
        var group = OrbitalGroup.CreateStationed(civilizationId, planetId, planetId, SpaceAssetType.ScoutCraft, 2);
        dbContext.Add(group);
        await dbContext.SaveChangesAsync();

        var profile = Assert.Single((await CreateService(dbContext).GetAsync(new GetSensorProfilesRequest(civilizationId))).Profiles);

        Assert.Equal(SensorProfileSourceKind.OrbitalGroup, profile.SourceKind);
        Assert.Equal(SensorProfileClass.Orbital, profile.SensorClass);
        Assert.Equal((group.Id, SpaceAssetType.ScoutCraft, 2, 15), (profile.OrbitalGroupId, profile.AssetType, profile.DetectionRangeTier, profile.ScanStrength));
    }

    [Fact]
    public async Task GetAsyncExcludesOtherCivilizationSourcesAndDoesNotMutateState()
    {
        await using var dbContext = CreateDbContext();
        var civilizationId = Guid.NewGuid();
        var otherCivilizationId = Guid.NewGuid();
        var system = CreateSystem("Shared");
        var planet = new Planet(Guid.NewGuid(), system.Id, "Home", 1, PlanetType.Oceanic, 70);
        var otherPlanet = new Planet(Guid.NewGuid(), system.Id, "Foreign", 2, PlanetType.Desert, 80);
        dbContext.AddRange(system, planet, otherPlanet, PlanetOwnership.Create(planet.Id, civilizationId),
            PlanetOwnership.Create(otherPlanet.Id, otherCivilizationId),
            OrbitalGroup.CreateStationed(otherCivilizationId, otherPlanet.Id, otherPlanet.Id, SpaceAssetType.ScoutCraft, 1),
            ExplorationKnowledge.Create(civilizationId, system.Id, planet.Id, ExplorationKnowledgeSource.MissionCompletion, Guid.NewGuid(), DateTime.UtcNow));
        await dbContext.SaveChangesAsync();
        var trackedCounts = (await dbContext.Set<PlanetOwnership>().CountAsync(), await dbContext.Set<ExplorationKnowledge>().CountAsync());

        var result = await CreateService(dbContext).GetAsync(new GetSensorProfilesRequest(civilizationId));

        Assert.Single(result.Profiles);
        Assert.All(result.Profiles, x => Assert.NotEqual(otherPlanet.Id, x.PlanetId));
        Assert.Equal(0, dbContext.ChangeTracker.Entries().Count(x => x.State != EntityState.Unchanged));
        Assert.Equal(trackedCounts, (await dbContext.Set<PlanetOwnership>().CountAsync(), await dbContext.Set<ExplorationKnowledge>().CountAsync()));
    }

    private static SensorProfileService CreateService(VoidEmpiresDbContext dbContext) => new(dbContext);

    private static SolarSystem CreateSystem(string name)
    {
        var systemId = Guid.NewGuid();
        return new SolarSystem(systemId, Guid.NewGuid(), name, new GalaxyCoordinates(1, 2, 3),
            new Star(Guid.NewGuid(), systemId, $"{name} Star", StarType.YellowDwarf));
    }

    private static VoidEmpiresDbContext CreateDbContext() =>
        new(new DbContextOptionsBuilder<VoidEmpiresDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
}
