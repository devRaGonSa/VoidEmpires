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

public class ExplorationToolingReadinessSmokeTests
{
    [Fact]
    public async Task ExplorationDevToolingLifecycleSurfacesRemainCoherentAndConservative()
    {
        await using var dbContext = CreateDbContext();
        var requestedAtUtc = new DateTime(2026, 6, 1, 12, 0, 0, DateTimeKind.Utc);
        var player = PlayerProfile.Create(Guid.NewGuid().ToString(), "Tooling Explorer");
        var civilization = Civilization.Create(player.Id, "Tooling Smoke", CivilizationArchetype.Exploratory);
        var homeSystem = CreateSystem("Home", 1, 2, 3);
        var targetSystem = CreateSystem("Veil", 4, 5, 6);
        var homePlanet = new Planet(Guid.NewGuid(), homeSystem.Id, "Homeworld", 1, PlanetType.Terran, 100, PlanetColonizationStatus.Colonized);
        var targetPlanet = new Planet(Guid.NewGuid(), targetSystem.Id, "Survey Target", 1, PlanetType.Barren, 60);
        var hiddenPlanet = new Planet(Guid.NewGuid(), targetSystem.Id, "Still Hidden", 2, PlanetType.Ice, 70);
        var stockpile = PlanetResourceStockpile.Create(homePlanet.Id);
        stockpile.Increase(ResourceType.Credits, 100);
        stockpile.Increase(ResourceType.Gas, 50);
        var group = OrbitalGroup.CreateStationed(civilization.Id, homePlanet.Id, homePlanet.Id, SpaceAssetType.ScoutCraft, 2);
        dbContext.PlayerProfiles.Add(player);
        dbContext.Civilizations.Add(civilization);
        dbContext.Set<SolarSystem>().AddRange(homeSystem, targetSystem);
        dbContext.Set<Planet>().AddRange(homePlanet, targetPlanet, hiddenPlanet);
        dbContext.Set<PlanetOwnership>().Add(PlanetOwnership.Create(homePlanet.Id, civilization.Id));
        dbContext.PlanetResourceStockpiles.Add(stockpile);
        dbContext.Set<OrbitalGroup>().Add(group);
        await dbContext.SaveChangesAsync();

        var visibilityService = new MapVisibilityService(dbContext);
        var previewService = new ExplorationActionPreviewService(visibilityService);
        var createService = new ExplorationMissionCreateService(dbContext, previewService);
        var completionService = new ExplorationMissionCompletionService(dbContext);
        var missionQueryService = new ExplorationMissionQueryService(dbContext);
        var knowledgeQueryService = new ExplorationKnowledgeQueryService(dbContext);

        var preview = await previewService.GetAsync(new GetExplorationActionPreviewRequest(civilization.Id));
        var targetPreview = preview.Systems.Single(x => x.SystemId == targetSystem.Id);
        Assert.True(targetPreview.CanPreviewSystemExploration);
        Assert.True(targetPreview.Planets.Single(x => x.PlanetId == targetPlanet.Id).CanPreviewPlanetExploration);

        var createResult = await createService.CreateAsync(new CreateExplorationMissionRequest(
            civilization.Id,
            targetSystem.Id,
            targetPlanet.Id,
            requestedAtUtc));

        Assert.True(createResult.Succeeded);
        var plannedMissionList = await missionQueryService.GetAsync(new GetExplorationMissionsRequest(civilization.Id, ExplorationMissionStatus.Planned));
        var plannedMission = Assert.Single(plannedMissionList.Missions);
        Assert.Equal(createResult.Mission!.ExplorationMissionId, plannedMission.ExplorationMissionId);
        Assert.Equal(ExplorationMissionStatus.Planned, plannedMission.Status);

        var completionResult = await completionService.CompleteDueAsync(
            new CompleteDueExplorationMissionsRequest(createResult.Mission.DueAtUtc));

        Assert.True(completionResult.Succeeded);
        Assert.Equal([createResult.Mission.ExplorationMissionId], completionResult.CompletedMissionIds);

        var knowledge = await knowledgeQueryService.GetAsync(new GetExplorationKnowledgeRequest(civilization.Id));
        Assert.True(knowledge.Succeeded);
        Assert.Equal(2, knowledge.Knowledge.Count);
        Assert.Contains(knowledge.Knowledge, x => x.SystemId == targetSystem.Id && x.PlanetId is null);
        Assert.Contains(knowledge.Knowledge, x => x.SystemId == targetSystem.Id && x.PlanetId == targetPlanet.Id);

        var completedMissionList = await missionQueryService.GetAsync(new GetExplorationMissionsRequest(civilization.Id, ExplorationMissionStatus.Completed));
        var completedMission = Assert.Single(completedMissionList.Missions);
        Assert.Equal(ExplorationMissionStatus.Completed, completedMission.Status);
        Assert.Equal(createResult.Mission.DueAtUtc, completedMission.CompletedAtUtc);

        var visibility = await visibilityService.GetAsync(new GetMapVisibilityRequest(civilization.Id));
        var targetVisibility = visibility.Systems.Single(x => x.SystemId == targetSystem.Id);
        Assert.Equal(MapVisibilityReason.ExploredSystem, targetVisibility.VisibilityReason);
        Assert.Equal(MapVisibilityReason.ExploredPlanet, targetVisibility.Planets.Single(x => x.PlanetId == targetPlanet.Id).VisibilityReason);
        Assert.Equal(MapVisibilityLevel.Unknown, targetVisibility.Planets.Single(x => x.PlanetId == hiddenPlanet.Id).VisibilityLevel);

        var strategicMap = await new StrategicMapService(
                dbContext,
                new SystemVisualStateService(dbContext, new PlanetVisualStateService(dbContext)),
                visibilityService)
            .GetAsync(new GetStrategicMapRequest(civilization.Id));
        var targetMapSystem = strategicMap.Systems.Single(x => x.SystemId == targetSystem.Id);
        Assert.False(targetMapSystem.ExplorationPreview.CanPreviewExploration);
        AssertBlocked(targetMapSystem.Commands, "exploration.mission.create", StrategicMapCommandBlockReason.ExplorationPreviewUnavailable);
        var hiddenMapPlanet = targetMapSystem.Planets.Single(x => x.PlanetId == hiddenPlanet.Id);
        Assert.Equal(MapVisibilityLevel.Unknown, hiddenMapPlanet.VisibilityLevel);
        Assert.Null(hiddenMapPlanet.PlanetName);
        Assert.True(hiddenMapPlanet.ExplorationPreview.CanPreviewExploration);
        AssertAvailable(hiddenMapPlanet.Commands, "exploration.mission.create");

        var manifestActions = new DevStrategicMapActionManifestService().Get().Actions;
        Assert.Contains(manifestActions, x => x.ActionKey == "exploration.preview.read" && x.IsReadOnly);
        Assert.Contains(manifestActions, x => x.ActionKey == "exploration.mission.create" && !x.IsReadOnly);
        Assert.Contains(manifestActions, x => x.ActionKey == "exploration.mission.completeDue" && !x.IsReadOnly);
        Assert.Contains(manifestActions, x => x.ActionKey == "exploration.knowledge.read" && x.IsReadOnly);
        Assert.Contains(manifestActions, x => x.ActionKey == "exploration.mission.list" && x.IsReadOnly);

        Assert.Equal(100, dbContext.PlanetResourceStockpiles.AsNoTracking().Single().Credits);
        Assert.Equal(50, dbContext.PlanetResourceStockpiles.AsNoTracking().Single().Gas);
        var persistedGroup = dbContext.Set<OrbitalGroup>().AsNoTracking().Single();
        Assert.Equal(OrbitalGroupStatus.Stationed, persistedGroup.Status);
        Assert.Equal(2, persistedGroup.Quantity);
        AssertNoFutureExplorationSystems(dbContext);
    }

    private static void AssertNoFutureExplorationSystems(VoidEmpiresDbContext dbContext)
    {
        var entityNames = dbContext.Model.GetEntityTypes().Select(x => x.ClrType.Name).ToArray();
        var blockedTerms = new[]
        {
            "Sensor", "Scanner", "Reward", "Combat", "Interception", "Espionage",
            "Diplomacy", "RouteGraph", "Pathfinding", "FinalUi"
        };

        foreach (var term in blockedTerms)
        {
            Assert.DoesNotContain(entityNames, x => x.Contains(term, StringComparison.OrdinalIgnoreCase));
        }
    }

    private static void AssertAvailable(IReadOnlyList<StrategicMapCommandAvailabilityDto> commands, string actionKey)
    {
        var command = commands.Single(x => x.ActionKey == actionKey);
        Assert.True(command.IsAvailable);
        Assert.Equal(StrategicMapCommandBlockReason.None, command.BlockReason);
    }

    private static void AssertBlocked(
        IReadOnlyList<StrategicMapCommandAvailabilityDto> commands,
        string actionKey,
        StrategicMapCommandBlockReason reason)
    {
        var command = commands.Single(x => x.ActionKey == actionKey);
        Assert.False(command.IsAvailable);
        Assert.Equal(reason, command.BlockReason);
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
