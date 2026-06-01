using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.StrategicMap;
using VoidEmpires.Domain.Exploration;
using VoidEmpires.Domain.Galaxy;
using VoidEmpires.Infrastructure.Persistence;
using VoidEmpires.Infrastructure.StrategicMap;

namespace VoidEmpires.Tests;

public class ExplorationMissionCompletionServiceTests
{
    [Fact]
    public async Task CompleteDueAsyncCompletesDuePlannedMission()
    {
        await using var dbContext = CreateDbContext();
        var nowUtc = new DateTime(2026, 6, 1, 12, 0, 0, DateTimeKind.Utc);
        var dueMission = ExplorationMission.CreatePlanned(
            Guid.NewGuid(),
            Guid.NewGuid(),
            null,
            nowUtc.AddHours(-1),
            nowUtc);
        dbContext.ExplorationMissions.Add(dueMission);
        await dbContext.SaveChangesAsync();

        var result = await new ExplorationMissionCompletionService(dbContext)
            .CompleteDueAsync(new CompleteDueExplorationMissionsRequest(nowUtc));

        Assert.True(result.Succeeded);
        Assert.Equal(1, result.CompletedCount);
        Assert.Equal([dueMission.Id], result.CompletedMissionIds);
        var persisted = await dbContext.ExplorationMissions.SingleAsync();
        var knowledge = await dbContext.ExplorationKnowledge.SingleAsync();
        Assert.Equal(ExplorationMissionStatus.Completed, persisted.Status);
        Assert.Equal(nowUtc, persisted.CompletedAtUtc);
        Assert.Equal((dueMission.CivilizationId, dueMission.TargetSystemId, (Guid?)null), (knowledge.CivilizationId, knowledge.SystemId, knowledge.PlanetId));
        Assert.Equal(ExplorationKnowledgeSource.MissionCompletion, knowledge.Source);
        Assert.Equal(dueMission.Id, knowledge.SourceMissionId);
    }

    [Fact]
    public async Task CompleteDueAsyncRecordsSystemAndPlanetKnowledgeForPlanetMission()
    {
        await using var dbContext = CreateDbContext();
        var nowUtc = new DateTime(2026, 6, 1, 12, 0, 0, DateTimeKind.Utc);
        var planetId = Guid.NewGuid();
        var mission = ExplorationMission.CreatePlanned(Guid.NewGuid(), Guid.NewGuid(), planetId, nowUtc.AddHours(-1), nowUtc);
        dbContext.ExplorationMissions.Add(mission);
        await dbContext.SaveChangesAsync();

        _ = await new ExplorationMissionCompletionService(dbContext)
            .CompleteDueAsync(new CompleteDueExplorationMissionsRequest(nowUtc));

        var knowledge = await dbContext.ExplorationKnowledge.OrderBy(x => x.PlanetId.HasValue).ToArrayAsync();
        Assert.Equal(2, knowledge.Length);
        Assert.Null(knowledge[0].PlanetId);
        Assert.Equal(planetId, knowledge[1].PlanetId);
        Assert.All(knowledge, x => Assert.Equal(mission.Id, x.SourceMissionId));
    }

    [Fact]
    public async Task CompleteDueAsyncDoesNotCompleteFutureMission()
    {
        await using var dbContext = CreateDbContext();
        var nowUtc = new DateTime(2026, 6, 1, 12, 0, 0, DateTimeKind.Utc);
        var futureMission = ExplorationMission.CreatePlanned(
            Guid.NewGuid(),
            Guid.NewGuid(),
            null,
            nowUtc,
            nowUtc.AddMinutes(1));
        dbContext.ExplorationMissions.Add(futureMission);
        await dbContext.SaveChangesAsync();

        var result = await new ExplorationMissionCompletionService(dbContext)
            .CompleteDueAsync(new CompleteDueExplorationMissionsRequest(nowUtc));

        Assert.True(result.Succeeded);
        Assert.Equal(0, result.CompletedCount);
        var persisted = await dbContext.ExplorationMissions.SingleAsync();
        Assert.Equal(ExplorationMissionStatus.Planned, persisted.Status);
        Assert.Null(persisted.CompletedAtUtc);
        Assert.Empty(await dbContext.ExplorationKnowledge.ToArrayAsync());
    }

    [Fact]
    public async Task CompleteDueAsyncDoesNotDuplicateKnowledge()
    {
        await using var dbContext = CreateDbContext();
        var nowUtc = new DateTime(2026, 6, 1, 12, 0, 0, DateTimeKind.Utc);
        var mission = ExplorationMission.CreatePlanned(Guid.NewGuid(), Guid.NewGuid(), null, nowUtc.AddHours(-1), nowUtc);
        dbContext.ExplorationMissions.Add(mission);
        await dbContext.SaveChangesAsync();

        var service = new ExplorationMissionCompletionService(dbContext);
        _ = await service.CompleteDueAsync(new CompleteDueExplorationMissionsRequest(nowUtc));
        _ = await service.CompleteDueAsync(new CompleteDueExplorationMissionsRequest(nowUtc.AddMinutes(1)));

        Assert.Equal(1, await dbContext.ExplorationKnowledge.CountAsync());
    }

    [Fact]
    public async Task CompleteDueAsyncRejectsNonUtcTimestamp()
    {
        await using var dbContext = CreateDbContext();

        var result = await new ExplorationMissionCompletionService(dbContext)
            .CompleteDueAsync(new CompleteDueExplorationMissionsRequest(new DateTime(2026, 6, 1, 12, 0, 0, DateTimeKind.Local)));

        Assert.False(result.Succeeded);
        Assert.Contains("Now must be UTC.", result.Errors);
    }

    [Fact]
    public async Task CompleteDueAsyncMakesCompletedTargetVisibleThroughKnowledge()
    {
        await using var dbContext = CreateDbContext();
        var civilizationId = Guid.NewGuid();
        var system = CreateSystem("Unknown", 1, 2, 3);
        var nowUtc = new DateTime(2026, 6, 1, 12, 0, 0, DateTimeKind.Utc);
        dbContext.Set<SolarSystem>().Add(system);
        dbContext.ExplorationMissions.Add(ExplorationMission.CreatePlanned(
            civilizationId,
            system.Id,
            null,
            nowUtc.AddHours(-1),
            nowUtc));
        await dbContext.SaveChangesAsync();

        _ = await new ExplorationMissionCompletionService(dbContext)
            .CompleteDueAsync(new CompleteDueExplorationMissionsRequest(nowUtc));

        var visibility = await new MapVisibilityService(dbContext).GetAsync(new GetMapVisibilityRequest(civilizationId));
        var mapSystem = Assert.Single(visibility.Systems);
        Assert.Equal(MapVisibilityLevel.Visible, mapSystem.VisibilityLevel);
        Assert.Equal(MapVisibilityReason.ExploredSystem, mapSystem.VisibilityReason);
        Assert.False(mapSystem.IsOwnedByRequestingCivilization);
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
