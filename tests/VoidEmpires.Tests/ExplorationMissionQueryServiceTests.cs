using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.StrategicMap;
using VoidEmpires.Domain.Exploration;
using VoidEmpires.Infrastructure.Persistence;
using VoidEmpires.Infrastructure.StrategicMap;

namespace VoidEmpires.Tests;

public class ExplorationMissionQueryServiceTests
{
    [Fact]
    public async Task GetAsyncScopesFiltersAndOrdersMissions()
    {
        await using var dbContext = CreateDbContext();
        var civilizationId = Guid.NewGuid();
        var otherCivilizationId = Guid.NewGuid();
        var requestedAtUtc = new DateTime(2026, 6, 1, 12, 0, 0, DateTimeKind.Utc);
        var completed = CreateMission(civilizationId, requestedAtUtc.AddMinutes(10), requestedAtUtc.AddMinutes(20));
        completed.Complete(requestedAtUtc.AddMinutes(20));
        var firstPlanned = CreateMission(civilizationId, requestedAtUtc, requestedAtUtc.AddMinutes(30));
        var secondPlanned = CreateMission(civilizationId, requestedAtUtc, requestedAtUtc.AddMinutes(45));
        dbContext.ExplorationMissions.AddRange(
            secondPlanned,
            CreateMission(otherCivilizationId, requestedAtUtc.AddMinutes(-5), requestedAtUtc.AddMinutes(30)),
            completed,
            firstPlanned);
        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();

        var all = await CreateService(dbContext).GetAsync(new GetExplorationMissionsRequest(civilizationId, null));
        var planned = await CreateService(dbContext).GetAsync(new GetExplorationMissionsRequest(civilizationId, ExplorationMissionStatus.Planned));

        Assert.True(all.Succeeded);
        Assert.Equal(3, all.Missions.Count);
        Assert.All(all.Missions, x => Assert.Equal(civilizationId, x.CivilizationId));
        Assert.Equal([firstPlanned.Id, secondPlanned.Id, completed.Id], all.Missions.Select(x => x.ExplorationMissionId).ToArray());
        Assert.Equal(2, planned.Missions.Count);
        Assert.All(planned.Missions, x => Assert.Equal(ExplorationMissionStatus.Planned, x.Status));
    }

    [Fact]
    public async Task GetAsyncRejectsEmptyCivilizationIdAndDoesNotMutateState()
    {
        await using var dbContext = CreateDbContext();
        var mission = CreateMission(Guid.NewGuid(), new DateTime(2026, 6, 1, 12, 0, 0, DateTimeKind.Utc), new DateTime(2026, 6, 1, 12, 30, 0, DateTimeKind.Utc));
        dbContext.ExplorationMissions.Add(mission);
        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();
        var countBefore = await dbContext.ExplorationMissions.CountAsync();

        var invalid = await CreateService(dbContext).GetAsync(new GetExplorationMissionsRequest(Guid.Empty, null));
        _ = await CreateService(dbContext).GetAsync(new GetExplorationMissionsRequest(mission.CivilizationId, null));

        Assert.False(invalid.Succeeded);
        Assert.Contains("Civilization id is required.", invalid.Errors);
        Assert.Equal(countBefore, await dbContext.ExplorationMissions.CountAsync());
        Assert.Empty(dbContext.ChangeTracker.Entries().Where(x => x.State != EntityState.Unchanged));
    }

    private static ExplorationMission CreateMission(Guid civilizationId, DateTime requestedAtUtc, DateTime dueAtUtc) =>
        ExplorationMission.CreatePlanned(civilizationId, Guid.NewGuid(), null, requestedAtUtc, dueAtUtc);

    private static ExplorationMissionQueryService CreateService(VoidEmpiresDbContext dbContext) => new(dbContext);

    private static VoidEmpiresDbContext CreateDbContext() =>
        new(new DbContextOptionsBuilder<VoidEmpiresDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);
}
