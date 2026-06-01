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

public class DetectionCoverageServiceTests
{
    [Fact]
    public async Task GetAsyncReturnsEmptyResultForCivilizationWithoutCoverageSources()
    {
        await using var dbContext = CreateDbContext();
        Assert.Empty((await CreateService(dbContext).GetAsync(new GetDetectionCoverageRequest(Guid.NewGuid()))).Coverages);
    }

    [Fact]
    public async Task GetAsyncReturnsOwnedPlanetLocalSystemCoverage()
    {
        await using var dbContext = CreateDbContext();
        var civilizationId = Guid.NewGuid();
        var system = CreateSystem("Home");
        var planet = new Planet(Guid.NewGuid(), system.Id, "Bastion", 1, PlanetType.Terran, 100, PlanetColonizationStatus.Colonized);
        dbContext.AddRange(system, planet, PlanetOwnership.Create(planet.Id, civilizationId));
        await dbContext.SaveChangesAsync();

        var coverage = Assert.Single((await CreateService(dbContext).GetAsync(new GetDetectionCoverageRequest(civilizationId))).Coverages);

        Assert.Equal(DetectionCoverageSourceKind.Planet, coverage.SourceKind);
        Assert.Equal(DetectionCoverageClass.Orbital, coverage.CoverageClass);
        Assert.Equal(system.Id, coverage.SourceSystemId);
        Assert.Equal([system.Id], coverage.CoveredSystemIds);
    }

    [Fact]
    public async Task GetAsyncReturnsScoutOrbitalGroupCurrentSystemCoverage()
    {
        await using var dbContext = CreateDbContext();
        var civilizationId = Guid.NewGuid();
        var system = CreateSystem("Scout Post");
        var planet = new Planet(Guid.NewGuid(), system.Id, "Relay", 1, PlanetType.Barren, 60);
        var scout = OrbitalGroup.CreateStationed(civilizationId, planet.Id, planet.Id, SpaceAssetType.ScoutCraft, 2);
        dbContext.AddRange(system, planet, scout);
        await dbContext.SaveChangesAsync();

        var coverage = Assert.Single((await CreateService(dbContext).GetAsync(new GetDetectionCoverageRequest(civilizationId))).Coverages);

        Assert.Equal(DetectionCoverageSourceKind.OrbitalGroup, coverage.SourceKind);
        Assert.Equal(DetectionCoverageClass.Orbital, coverage.CoverageClass);
        Assert.Equal(scout.Id, coverage.OrbitalGroupId);
        Assert.Equal([system.Id], coverage.CoveredSystemIds);
    }

    [Fact]
    public async Task GetAsyncExcludesOtherCivilizationDataAndUnknownTargets()
    {
        await using var dbContext = CreateDbContext();
        var civilizationId = Guid.NewGuid();
        var otherCivilizationId = Guid.NewGuid();
        var homeSystem = CreateSystem("Home");
        var hiddenSystem = CreateSystem("Hidden");
        var homePlanet = new Planet(Guid.NewGuid(), homeSystem.Id, "Bastion", 1, PlanetType.Terran, 100, PlanetColonizationStatus.Colonized);
        var hiddenPlanet = new Planet(Guid.NewGuid(), hiddenSystem.Id, "Shadow", 1, PlanetType.Desert, 90);
        dbContext.AddRange(
            homeSystem,
            hiddenSystem,
            homePlanet,
            hiddenPlanet,
            PlanetOwnership.Create(homePlanet.Id, civilizationId),
            PlanetOwnership.Create(hiddenPlanet.Id, otherCivilizationId),
            OrbitalGroup.CreateStationed(otherCivilizationId, hiddenPlanet.Id, hiddenPlanet.Id, SpaceAssetType.ScoutCraft, 1),
            ExplorationKnowledge.Create(civilizationId, homeSystem.Id, homePlanet.Id, ExplorationKnowledgeSource.MissionCompletion, Guid.NewGuid(), DateTime.UtcNow));
        await dbContext.SaveChangesAsync();

        var coverages = (await CreateService(dbContext).GetAsync(new GetDetectionCoverageRequest(civilizationId))).Coverages;

        var coverage = Assert.Single(coverages);
        Assert.Equal(homePlanet.Id, coverage.SourcePlanetId);
        Assert.DoesNotContain(hiddenSystem.Id, coverage.CoveredSystemIds);
        Assert.DoesNotContain(coverages, x => x.SourcePlanetId == hiddenPlanet.Id || x.SourceSystemId == hiddenSystem.Id);
    }

    [Fact]
    public async Task GetAsyncRemainsReadOnly()
    {
        await using var dbContext = CreateDbContext();
        var civilizationId = Guid.NewGuid();
        var system = CreateSystem("Stable");
        var planet = new Planet(Guid.NewGuid(), system.Id, "Anchor", 1, PlanetType.Oceanic, 80, PlanetColonizationStatus.Colonized);
        dbContext.AddRange(system, planet, PlanetOwnership.Create(planet.Id, civilizationId));
        await dbContext.SaveChangesAsync();
        var counts = (await dbContext.PlanetOwnerships.CountAsync(), await dbContext.ExplorationKnowledge.CountAsync());

        _ = await CreateService(dbContext).GetAsync(new GetDetectionCoverageRequest(civilizationId));

        Assert.Equal(0, dbContext.ChangeTracker.Entries().Count(x => x.State != EntityState.Unchanged));
        Assert.Equal(counts, (await dbContext.PlanetOwnerships.CountAsync(), await dbContext.ExplorationKnowledge.CountAsync()));
    }

    private static DetectionCoverageService CreateService(VoidEmpiresDbContext dbContext) =>
        new(dbContext, new SensorProfileService(dbContext));

    private static SolarSystem CreateSystem(string name)
    {
        var systemId = Guid.NewGuid();
        return new SolarSystem(systemId, Guid.NewGuid(), name, new GalaxyCoordinates(1, 2, 3),
            new Star(Guid.NewGuid(), systemId, $"{name} Star", StarType.YellowDwarf));
    }

    private static VoidEmpiresDbContext CreateDbContext() =>
        new(new DbContextOptionsBuilder<VoidEmpiresDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
}
