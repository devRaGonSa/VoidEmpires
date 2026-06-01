using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.StrategicMap;
using VoidEmpires.Application.Visuals;
using VoidEmpires.Domain.Assets;
using VoidEmpires.Domain.Colonization;
using VoidEmpires.Domain.Economy;
using VoidEmpires.Domain.Exploration;
using VoidEmpires.Domain.Fleets;
using VoidEmpires.Domain.Galaxy;
using VoidEmpires.Domain.Players;
using VoidEmpires.Infrastructure.Persistence;
using VoidEmpires.Infrastructure.StrategicMap;
using VoidEmpires.Infrastructure.Visuals;

namespace VoidEmpires.Tests;

public class DetectionReadinessSmokeTests
{
    [Fact]
    public async Task DetectionReadinessSurfacesRemainCoherentConservativeAndReadOnly()
    {
        await using var dbContext = CreateDbContext();
        var requestedAtUtc = new DateTime(2026, 6, 1, 15, 0, 0, DateTimeKind.Utc);
        var player = PlayerProfile.Create(Guid.NewGuid().ToString(), "Detection Smoke");
        var civilization = Civilization.Create(player.Id, "Detection Readiness", CivilizationArchetype.Exploratory);
        var homeSystem = CreateSystem("Home", 1, 2, 3);
        var targetSystem = CreateSystem("Signal Veil", 4, 5, 6);
        var homePlanet = new Planet(Guid.NewGuid(), homeSystem.Id, "Bastion", 1, PlanetType.Terran, 100, PlanetColonizationStatus.Colonized);
        var targetPlanet = new Planet(Guid.NewGuid(), targetSystem.Id, "Survey Target", 1, PlanetType.Barren, 60);
        var hiddenPlanet = new Planet(Guid.NewGuid(), targetSystem.Id, "Still Hidden", 2, PlanetType.Ice, 70);
        var stockpile = PlanetResourceStockpile.Create(homePlanet.Id);
        stockpile.Increase(ResourceType.Credits, 100);
        stockpile.Increase(ResourceType.Gas, 50);
        var scout = OrbitalGroup.CreateStationed(civilization.Id, homePlanet.Id, homePlanet.Id, SpaceAssetType.ScoutCraft, 2);
        dbContext.PlayerProfiles.Add(player);
        dbContext.Civilizations.Add(civilization);
        dbContext.Set<SolarSystem>().AddRange(homeSystem, targetSystem);
        dbContext.Set<Planet>().AddRange(homePlanet, targetPlanet, hiddenPlanet);
        dbContext.Set<PlanetOwnership>().Add(PlanetOwnership.Create(homePlanet.Id, civilization.Id));
        dbContext.PlanetResourceStockpiles.Add(stockpile);
        dbContext.Set<OrbitalGroup>().Add(scout);
        dbContext.ExplorationKnowledge.Add(ExplorationKnowledge.Create(
            civilization.Id,
            targetSystem.Id,
            targetPlanet.Id,
            ExplorationKnowledgeSource.MissionCompletion,
            Guid.NewGuid(),
            requestedAtUtc));
        await dbContext.SaveChangesAsync();

        var sensorProfileService = new SensorProfileService(dbContext);
        var detectionCoverageService = new DetectionCoverageService(dbContext, sensorProfileService);
        var visibilityService = new MapVisibilityService(dbContext);
        var strategicMapService = new StrategicMapService(
            dbContext,
            new SystemVisualStateService(dbContext, new PlanetVisualStateService(dbContext)),
            visibilityService,
            sensorProfileService,
            detectionCoverageService);

        var sensorProfiles = await sensorProfileService.GetAsync(new GetSensorProfilesRequest(civilization.Id));
        Assert.Equal(2, sensorProfiles.Profiles.Count);
        Assert.Contains(sensorProfiles.Profiles, x => x.SourceKind == SensorProfileSourceKind.Planet && x.PlanetId == homePlanet.Id);
        Assert.Contains(sensorProfiles.Profiles, x => x.SourceKind == SensorProfileSourceKind.OrbitalGroup && x.OrbitalGroupId == scout.Id);

        var detectionCoverage = await detectionCoverageService.GetAsync(new GetDetectionCoverageRequest(civilization.Id));
        Assert.Equal(2, detectionCoverage.Coverages.Count);
        Assert.Contains(detectionCoverage.Coverages, x => x.SourceKind == DetectionCoverageSourceKind.Planet && x.SourcePlanetId == homePlanet.Id);
        Assert.Contains(detectionCoverage.Coverages, x => x.SourceKind == DetectionCoverageSourceKind.OrbitalGroup && x.OrbitalGroupId == scout.Id);
        Assert.All(detectionCoverage.Coverages, x => Assert.Equal([homeSystem.Id], x.CoveredSystemIds));

        var visibility = await visibilityService.GetAsync(new GetMapVisibilityRequest(civilization.Id));
        var visibleHomeSystem = visibility.Systems.Single(x => x.SystemId == homeSystem.Id);
        Assert.Equal(MapVisibilityLevel.Visible, visibleHomeSystem.VisibilityLevel);
        var exploredTargetSystem = visibility.Systems.Single(x => x.SystemId == targetSystem.Id);
        Assert.Equal(MapVisibilityReason.ExploredSystem, exploredTargetSystem.VisibilityReason);
        Assert.Equal(MapVisibilityLevel.Unknown, exploredTargetSystem.Planets.Single(x => x.PlanetId == hiddenPlanet.Id).VisibilityLevel);

        var strategicMap = await strategicMapService.GetAsync(new GetStrategicMapRequest(civilization.Id));
        Assert.Contains(strategicMap.DetectionNotes, x => x.Note.Contains("does not reveal unknown systems or planets", StringComparison.OrdinalIgnoreCase));
        var homeMapSystem = strategicMap.Systems.Single(x => x.SystemId == homeSystem.Id);
        Assert.Contains(homeMapSystem.SensorProfiles, x => x.SourceKind == SensorProfileSourceKind.Planet);
        Assert.Contains(homeMapSystem.DetectionCoverage, x => x.SourceKind == DetectionCoverageSourceKind.Planet);
        Assert.Contains(homeMapSystem.DetectionCoverage, x => x.SourceKind == DetectionCoverageSourceKind.OrbitalGroup);
        Assert.Contains(homeMapSystem.FleetPresence, x => x.SensorProfile?.SourceKind == SensorProfileSourceKind.OrbitalGroup);
        Assert.Contains(homeMapSystem.Planets.Single(x => x.PlanetId == homePlanet.Id).DetectionCoverage,
            x => x.SourceKind == DetectionCoverageSourceKind.Planet);
        var targetMapSystem = strategicMap.Systems.Single(x => x.SystemId == targetSystem.Id);
        Assert.Empty(targetMapSystem.DetectionCoverage);
        var hiddenMapPlanet = targetMapSystem.Planets.Single(x => x.PlanetId == hiddenPlanet.Id);
        Assert.Null(hiddenMapPlanet.PlanetName);
        Assert.Equal(MapVisibilityLevel.Unknown, hiddenMapPlanet.VisibilityLevel);
        Assert.Empty(hiddenMapPlanet.SensorProfiles);
        Assert.Empty(hiddenMapPlanet.DetectionCoverage);

        var manifestActions = new DevStrategicMapActionManifestService().Get().Actions;
        Assert.Contains(manifestActions, x => x.ActionKey == "detection.coverage.read" && x.IsReadOnly);
        Assert.Contains(manifestActions, x => x.ActionKey == "sensor.profile.read" && x.IsReadOnly);

        Assert.Equal(100, dbContext.PlanetResourceStockpiles.AsNoTracking().Single().Credits);
        Assert.Equal(50, dbContext.PlanetResourceStockpiles.AsNoTracking().Single().Gas);
        Assert.Equal(1, dbContext.ExplorationKnowledge.AsNoTracking().Count());
        var persistedGroup = dbContext.Set<OrbitalGroup>().AsNoTracking().Single();
        Assert.Equal(OrbitalGroupStatus.Stationed, persistedGroup.Status);
        Assert.Equal(2, persistedGroup.Quantity);
        AssertNoFutureDetectionSystems(dbContext);
    }

    private static void AssertNoFutureDetectionSystems(VoidEmpiresDbContext dbContext)
    {
        var entityNames = dbContext.Model.GetEntityTypes().Select(x => x.ClrType.Name).ToArray();
        var blockedTerms = new[]
        {
            "DetectionState", "Scanner", "Combat", "Interception", "Espionage",
            "Diplomacy", "RouteGraph", "Pathfinding", "FinalUi"
        };

        foreach (var term in blockedTerms)
        {
            Assert.DoesNotContain(entityNames, x => x.Contains(term, StringComparison.OrdinalIgnoreCase));
        }
    }

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
