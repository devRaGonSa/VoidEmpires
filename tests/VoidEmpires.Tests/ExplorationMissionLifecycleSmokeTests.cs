using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.StrategicMap;
using VoidEmpires.Application.Visuals;
using VoidEmpires.Domain.Assets;
using VoidEmpires.Domain.Buildings;
using VoidEmpires.Domain.Colonization;
using VoidEmpires.Domain.Economy;
using VoidEmpires.Domain.Exploration;
using VoidEmpires.Domain.Fleets;
using VoidEmpires.Domain.Galaxy;
using VoidEmpires.Domain.Players;
using VoidEmpires.Infrastructure.Fleets;
using VoidEmpires.Infrastructure.Persistence;
using VoidEmpires.Infrastructure.StrategicMap;
using VoidEmpires.Infrastructure.Visuals;

namespace VoidEmpires.Tests;

public class ExplorationMissionLifecycleSmokeTests
{
    [Fact]
    public async Task PreviewCreateAndCompleteLifecycleRecordsKnowledgeAndRevealsVisibilityWithoutMutatingFleetResources()
    {
        await using var dbContext = CreateDbContext();
        var requestedAtUtc = new DateTime(2026, 6, 1, 12, 0, 0, DateTimeKind.Utc);
        var player = PlayerProfile.Create(Guid.NewGuid().ToString(), "Explorer");
        var civilization = Civilization.Create(player.Id, "Exploration Smoke", CivilizationArchetype.Exploratory);
        var otherCivilizationId = Guid.NewGuid();
        var homeSystem = CreateSystem("Home", 1, 2, 3);
        var targetSystem = CreateSystem("Unknown", 4, 5, 6);
        var homePlanet = new Planet(Guid.NewGuid(), homeSystem.Id, "Homeworld", 1, PlanetType.Terran, 100, PlanetColonizationStatus.Colonized);
        var targetPlanet = new Planet(Guid.NewGuid(), targetSystem.Id, "Veiled", 1, PlanetType.Barren, 60);
        var foreignPlanet = new Planet(Guid.NewGuid(), targetSystem.Id, "Foreign", 2, PlanetType.Desert, 90);
        var group = OrbitalGroup.CreateStationed(civilization.Id, homePlanet.Id, homePlanet.Id, SpaceAssetType.ScoutCraft, 2);
        group.Reserve();
        var transfer = OrbitalTransfer.CreatePlanned(
            civilization.Id,
            group.Id,
            homePlanet.Id,
            targetPlanet.Id,
            2,
            requestedAtUtc,
            requestedAtUtc.AddHours(2));
        var stockpile = PlanetResourceStockpile.Create(homePlanet.Id);
        stockpile.Increase(ResourceType.Credits, 100);
        stockpile.Increase(ResourceType.Gas, 50);
        dbContext.PlayerProfiles.Add(player);
        dbContext.Civilizations.Add(civilization);
        dbContext.Set<SolarSystem>().AddRange(homeSystem, targetSystem);
        dbContext.Set<Planet>().AddRange(homePlanet, targetPlanet, foreignPlanet);
        dbContext.Set<PlanetOwnership>().AddRange(
            PlanetOwnership.Create(homePlanet.Id, civilization.Id),
            PlanetOwnership.Create(foreignPlanet.Id, otherCivilizationId));
        dbContext.Set<PlanetBuilding>().Add(PlanetBuilding.Create(foreignPlanet.Id, BuildingType.CommandCenter, 1, 5));
        dbContext.Set<OrbitalGroup>().Add(group);
        dbContext.Set<OrbitalTransfer>().Add(transfer);
        dbContext.PlanetResourceStockpiles.Add(stockpile);
        await dbContext.SaveChangesAsync();

        var previewService = new ExplorationActionPreviewService(new MapVisibilityService(dbContext));
        var preview = await previewService.GetAsync(new GetExplorationActionPreviewRequest(civilization.Id));

        var targetPreview = preview.Systems.Single(x => x.SystemId == targetSystem.Id);
        Assert.True(targetPreview.CanPreviewSystemExploration);
        Assert.True(targetPreview.Planets.Single(x => x.PlanetId == targetPlanet.Id).CanPreviewPlanetExploration);

        var createResult = await new ExplorationMissionCreateService(dbContext, previewService)
            .CreateAsync(new CreateExplorationMissionRequest(
                civilization.Id,
                targetSystem.Id,
                targetPlanet.Id,
                requestedAtUtc));

        Assert.True(createResult.Succeeded);
        Assert.NotNull(createResult.Mission);
        Assert.Equal(ExplorationMissionStatus.Planned, createResult.Mission.Status);
        Assert.Equal(requestedAtUtc.AddMinutes(45), createResult.Mission.DueAtUtc);

        var completionResult = await new ExplorationMissionCompletionService(dbContext)
            .CompleteDueAsync(new CompleteDueExplorationMissionsRequest(createResult.Mission.DueAtUtc));

        Assert.True(completionResult.Succeeded);
        Assert.Equal([createResult.Mission.ExplorationMissionId], completionResult.CompletedMissionIds);
        var mission = await dbContext.ExplorationMissions.SingleAsync();
        Assert.Equal(ExplorationMissionStatus.Completed, mission.Status);
        Assert.Equal(createResult.Mission.DueAtUtc, mission.CompletedAtUtc);
        var knowledge = await dbContext.ExplorationKnowledge.OrderBy(x => x.PlanetId.HasValue).ToArrayAsync();
        Assert.Equal(2, knowledge.Length);
        Assert.Null(knowledge[0].PlanetId);
        Assert.Equal(targetPlanet.Id, knowledge[1].PlanetId);

        var visibility = await new MapVisibilityService(dbContext).GetAsync(new GetMapVisibilityRequest(civilization.Id));
        var targetVisibility = visibility.Systems.Single(x => x.SystemId == targetSystem.Id);
        Assert.Equal(MapVisibilityLevel.Visible, targetVisibility.VisibilityLevel);
        Assert.Equal(MapVisibilityReason.ExploredSystem, targetVisibility.VisibilityReason);
        Assert.True(targetVisibility.IsVisible);
        var targetVisibilityPlanet = targetVisibility.Planets.Single(x => x.PlanetId == targetPlanet.Id);
        Assert.Equal(MapVisibilityReason.ExploredPlanet, targetVisibilityPlanet.VisibilityReason);
        Assert.False(targetVisibilityPlanet.IsOwnedByRequestingCivilization);
        var foreignVisibilityPlanet = targetVisibility.Planets.Single(x => x.PlanetId == foreignPlanet.Id);
        Assert.Equal(MapVisibilityLevel.Unknown, foreignVisibilityPlanet.VisibilityLevel);
        Assert.Null(foreignVisibilityPlanet.CivilizationId);

        var systemVisualService = new SystemVisualStateService(
            dbContext,
            new PlanetVisualStateService(dbContext));
        var strategicMap = await new StrategicMapService(dbContext, systemVisualService, new MapVisibilityService(dbContext))
            .GetAsync(new GetStrategicMapRequest(civilization.Id));
        var targetMapSystem = strategicMap.Systems.Single(x => x.SystemId == targetSystem.Id);
        Assert.Equal(MapVisibilityLevel.Visible, targetMapSystem.VisibilityLevel);
        Assert.True(targetMapSystem.IsVisible);
        Assert.False(targetMapSystem.ExplorationPreview.CanPreviewExploration);
        var targetMapPlanet = targetMapSystem.Planets.Single(x => x.PlanetId == targetPlanet.Id);
        Assert.Equal(MapVisibilityReason.ExploredPlanet, targetMapPlanet.VisibilityReason);
        Assert.False(targetMapPlanet.IsOwnedByRequestingCivilization);
        var foreignMapPlanet = targetMapSystem.Planets.Single(x => x.PlanetId == foreignPlanet.Id);
        Assert.Equal(MapVisibilityLevel.Unknown, foreignMapPlanet.VisibilityLevel);
        Assert.False(foreignMapPlanet.IsOwnedByRequestingCivilization);
        Assert.Null(foreignMapPlanet.CivilizationId);
        Assert.Null(foreignMapPlanet.PlanetName);
        Assert.Null(foreignMapPlanet.PlanetType);
        Assert.Null(foreignMapPlanet.Size);
        Assert.Null(foreignMapPlanet.ColonizationIntensity);
        Assert.True(foreignMapPlanet.ExplorationPreview.CanPreviewExploration);

        var revealedPreview = await previewService.GetAsync(new GetExplorationActionPreviewRequest(civilization.Id));
        var revealedTargetPreview = revealedPreview.Systems.Single(x => x.SystemId == targetSystem.Id);
        Assert.False(revealedTargetPreview.CanPreviewSystemExploration);
        Assert.False(revealedTargetPreview.Planets.Single(x => x.PlanetId == targetPlanet.Id).CanPreviewPlanetExploration);

        Assert.Equal(100, (await dbContext.PlanetResourceStockpiles.SingleAsync()).Credits);
        Assert.Equal(50, (await dbContext.PlanetResourceStockpiles.SingleAsync()).Gas);
        var persistedGroup = await dbContext.Set<OrbitalGroup>().SingleAsync();
        Assert.Equal(OrbitalGroupStatus.Reserved, persistedGroup.Status);
        Assert.Equal(2, persistedGroup.Quantity);
        Assert.Equal(OrbitalTransferStatus.Planned, (await dbContext.Set<OrbitalTransfer>().SingleAsync()).Status);
        AssertNoFutureExplorationPersistence(dbContext);
    }

    private static void AssertNoFutureExplorationPersistence(VoidEmpiresDbContext dbContext)
    {
        var entityNames = dbContext.Model.GetEntityTypes().Select(x => x.ClrType.Name).ToArray();
        Assert.DoesNotContain(entityNames, x => x.Contains("Known", StringComparison.OrdinalIgnoreCase));
        Assert.DoesNotContain(entityNames, x => x.Contains("Fog", StringComparison.OrdinalIgnoreCase));
        Assert.DoesNotContain(entityNames, x => x.Contains("Sensor", StringComparison.OrdinalIgnoreCase));
        Assert.DoesNotContain(entityNames, x => x.Contains("Scanner", StringComparison.OrdinalIgnoreCase));
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
