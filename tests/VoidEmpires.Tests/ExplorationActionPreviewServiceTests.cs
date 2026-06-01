using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.StrategicMap;
using VoidEmpires.Domain.Colonization;
using VoidEmpires.Domain.Exploration;
using VoidEmpires.Domain.Galaxy;
using VoidEmpires.Infrastructure.Persistence;
using VoidEmpires.Infrastructure.StrategicMap;

namespace VoidEmpires.Tests;

public class ExplorationActionPreviewServiceTests
{
    [Fact]
    public async Task GetAsyncAllowsPreviewForUnknownSystemsAndPlanets()
    {
        await using var dbContext = CreateDbContext();
        var civilizationId = Guid.NewGuid();
        var unknownSystem = CreateSystem("Unknown", 1, 2, 3);
        var unknownPlanet = new Planet(Guid.NewGuid(), unknownSystem.Id, "Hidden", 1, PlanetType.Barren, 60);
        dbContext.Set<SolarSystem>().Add(unknownSystem);
        dbContext.Set<Planet>().Add(unknownPlanet);
        await dbContext.SaveChangesAsync();

        var result = await CreateService(dbContext).GetAsync(new GetExplorationActionPreviewRequest(civilizationId));

        var systemPreview = Assert.Single(result.Systems);
        Assert.Equal(unknownSystem.Id, systemPreview.SystemId);
        Assert.Equal(MapVisibilityLevel.Unknown, systemPreview.VisibilityLevel);
        Assert.True(systemPreview.CanPreviewSystemExploration);
        Assert.Equal(ExplorationActionBlockReason.None, systemPreview.BlockReason);
        var planetPreview = Assert.Single(systemPreview.Planets);
        Assert.True(planetPreview.CanPreviewPlanetExploration);
        Assert.Equal(ExplorationActionBlockReason.None, planetPreview.BlockReason);
        Assert.Contains(result.Notes, x => x.ActionKey == "exploration.preview" && x.IsReadOnly);
    }

    [Fact]
    public async Task GetAsyncBlocksPreviewForVisibleAndOwnedTargets()
    {
        await using var dbContext = CreateDbContext();
        var civilizationId = Guid.NewGuid();
        var system = CreateSystem("Visible", 4, 5, 6);
        var ownedPlanet = new Planet(Guid.NewGuid(), system.Id, "Home", 1, PlanetType.Terran, 100, PlanetColonizationStatus.Colonized);
        var visiblePlanet = new Planet(Guid.NewGuid(), system.Id, "Neighbor", 2, PlanetType.Ice, 80);
        dbContext.Set<SolarSystem>().Add(system);
        dbContext.Set<Planet>().AddRange(ownedPlanet, visiblePlanet);
        dbContext.Set<PlanetOwnership>().Add(PlanetOwnership.Create(ownedPlanet.Id, civilizationId));
        await dbContext.SaveChangesAsync();

        var result = await CreateService(dbContext).GetAsync(new GetExplorationActionPreviewRequest(civilizationId));

        var systemPreview = Assert.Single(result.Systems);
        Assert.False(systemPreview.CanPreviewSystemExploration);
        Assert.Equal(ExplorationActionBlockReason.AlreadyVisible, systemPreview.BlockReason);
        var ownedPreview = systemPreview.Planets.Single(x => x.PlanetId == ownedPlanet.Id);
        Assert.False(ownedPreview.CanPreviewPlanetExploration);
        Assert.Equal(ExplorationActionBlockReason.AlreadyOwned, ownedPreview.BlockReason);
        var visiblePreview = systemPreview.Planets.Single(x => x.PlanetId == visiblePlanet.Id);
        Assert.False(visiblePreview.CanPreviewPlanetExploration);
        Assert.Equal(ExplorationActionBlockReason.AlreadyVisible, visiblePreview.BlockReason);
    }

    [Fact]
    public async Task GetAsyncBlocksPreviewForExploredTargets()
    {
        await using var dbContext = CreateDbContext();
        var civilizationId = Guid.NewGuid();
        var system = CreateSystem("Explored", 4, 5, 6);
        var planet = new Planet(Guid.NewGuid(), system.Id, "Known", 1, PlanetType.Ice, 80);
        dbContext.Set<SolarSystem>().Add(system);
        dbContext.Set<Planet>().Add(planet);
        dbContext.ExplorationKnowledge.Add(ExplorationKnowledge.Create(civilizationId, system.Id, planet.Id, ExplorationKnowledgeSource.MissionCompletion, Guid.NewGuid(), DateTime.UtcNow));
        await dbContext.SaveChangesAsync();

        var preview = Assert.Single((await CreateService(dbContext).GetAsync(new GetExplorationActionPreviewRequest(civilizationId))).Systems);

        Assert.False(preview.CanPreviewSystemExploration);
        Assert.Equal(ExplorationActionBlockReason.AlreadyVisible, preview.BlockReason);
        Assert.False(Assert.Single(preview.Planets).CanPreviewPlanetExploration);
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
        await dbContext.SaveChangesAsync();
        var systemCount = await dbContext.Set<SolarSystem>().CountAsync();
        var planetCount = await dbContext.Set<Planet>().CountAsync();
        var ownershipCount = await dbContext.Set<PlanetOwnership>().CountAsync();

        _ = await CreateService(dbContext).GetAsync(new GetExplorationActionPreviewRequest(civilizationId));

        Assert.Equal(systemCount, await dbContext.Set<SolarSystem>().CountAsync());
        Assert.Equal(planetCount, await dbContext.Set<Planet>().CountAsync());
        Assert.Equal(ownershipCount, await dbContext.Set<PlanetOwnership>().CountAsync());
        Assert.DoesNotContain(dbContext.ChangeTracker.Entries(), x => x.State is EntityState.Added or EntityState.Modified or EntityState.Deleted);
    }

    private static ExplorationActionPreviewService CreateService(VoidEmpiresDbContext dbContext) =>
        new(new MapVisibilityService(dbContext));

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
