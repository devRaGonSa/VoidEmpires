using Microsoft.EntityFrameworkCore;
using VoidEmpires.Domain.Exploration;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Tests;

public class ExplorationMissionTests
{
    [Fact]
    public void CreatePlannedCreatesValidMission()
    {
        var civilizationId = Guid.NewGuid();
        var targetSystemId = Guid.NewGuid();
        var targetPlanetId = Guid.NewGuid();
        var requestedAtUtc = new DateTime(2026, 6, 1, 12, 0, 0, DateTimeKind.Utc);
        var dueAtUtc = requestedAtUtc.AddHours(4);

        var mission = ExplorationMission.CreatePlanned(
            civilizationId,
            targetSystemId,
            targetPlanetId,
            requestedAtUtc,
            dueAtUtc);

        Assert.NotEqual(Guid.Empty, mission.Id);
        Assert.Equal(civilizationId, mission.CivilizationId);
        Assert.Equal(targetSystemId, mission.TargetSystemId);
        Assert.Equal(targetPlanetId, mission.TargetPlanetId);
        Assert.Equal(requestedAtUtc, mission.RequestedAtUtc);
        Assert.Equal(dueAtUtc, mission.DueAtUtc);
        Assert.Null(mission.CompletedAtUtc);
        Assert.Equal(ExplorationMissionStatus.Planned, mission.Status);
    }

    [Fact]
    public void CreatePlannedAllowsSystemTargetWithoutPlanet()
    {
        var requestedAtUtc = new DateTime(2026, 6, 1, 12, 0, 0, DateTimeKind.Utc);

        var mission = ExplorationMission.CreatePlanned(
            Guid.NewGuid(),
            Guid.NewGuid(),
            null,
            requestedAtUtc,
            requestedAtUtc);

        Assert.Null(mission.TargetPlanetId);
    }

    [Fact]
    public void CreatePlannedRejectsInvalidIds()
    {
        var requestedAtUtc = new DateTime(2026, 6, 1, 12, 0, 0, DateTimeKind.Utc);

        Assert.Throws<ArgumentException>(() => ExplorationMission.CreatePlanned(
            Guid.Empty,
            Guid.NewGuid(),
            null,
            requestedAtUtc,
            requestedAtUtc));

        Assert.Throws<ArgumentException>(() => ExplorationMission.CreatePlanned(
            Guid.NewGuid(),
            Guid.Empty,
            null,
            requestedAtUtc,
            requestedAtUtc));

        Assert.Throws<ArgumentException>(() => ExplorationMission.CreatePlanned(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.Empty,
            requestedAtUtc,
            requestedAtUtc));
    }

    [Fact]
    public void CreatePlannedRequiresUtcDates()
    {
        var requestedAtUtc = new DateTime(2026, 6, 1, 12, 0, 0, DateTimeKind.Utc);
        var localDueAt = new DateTime(2026, 6, 1, 13, 0, 0, DateTimeKind.Local);

        Assert.Throws<ArgumentException>(() => ExplorationMission.CreatePlanned(
            Guid.NewGuid(),
            Guid.NewGuid(),
            null,
            requestedAtUtc,
            localDueAt));
    }

    [Fact]
    public void CreatePlannedRequiresDueAtOrAfterRequestedAt()
    {
        var requestedAtUtc = new DateTime(2026, 6, 1, 12, 0, 0, DateTimeKind.Utc);

        Assert.Throws<ArgumentException>(() => ExplorationMission.CreatePlanned(
            Guid.NewGuid(),
            Guid.NewGuid(),
            null,
            requestedAtUtc,
            requestedAtUtc.AddTicks(-1)));
    }

    [Fact]
    public void CompleteMarksMissionCompleted()
    {
        var requestedAtUtc = new DateTime(2026, 6, 1, 12, 0, 0, DateTimeKind.Utc);
        var completedAtUtc = requestedAtUtc.AddHours(4);
        var mission = ExplorationMission.CreatePlanned(
            Guid.NewGuid(),
            Guid.NewGuid(),
            null,
            requestedAtUtc,
            completedAtUtc);

        mission.Complete(completedAtUtc);

        Assert.Equal(ExplorationMissionStatus.Completed, mission.Status);
        Assert.Equal(completedAtUtc, mission.CompletedAtUtc);
    }

    [Fact]
    public void CompleteRejectsDoubleCompletion()
    {
        var requestedAtUtc = new DateTime(2026, 6, 1, 12, 0, 0, DateTimeKind.Utc);
        var mission = ExplorationMission.CreatePlanned(
            Guid.NewGuid(),
            Guid.NewGuid(),
            null,
            requestedAtUtc,
            requestedAtUtc);
        mission.Complete(requestedAtUtc);

        Assert.Throws<InvalidOperationException>(() => mission.Complete(requestedAtUtc));
    }

    [Fact]
    public async Task DbContextPersistsAndReadsBackMission()
    {
        await using var dbContext = CreateDbContext();
        var requestedAtUtc = new DateTime(2026, 6, 1, 12, 0, 0, DateTimeKind.Utc);
        var mission = ExplorationMission.CreatePlanned(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            requestedAtUtc,
            requestedAtUtc.AddHours(2));

        dbContext.ExplorationMissions.Add(mission);
        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();

        var persistedMission = await dbContext.ExplorationMissions.SingleAsync();

        Assert.Equal(mission.Id, persistedMission.Id);
        Assert.Equal(mission.CivilizationId, persistedMission.CivilizationId);
        Assert.Equal(mission.TargetSystemId, persistedMission.TargetSystemId);
        Assert.Equal(mission.TargetPlanetId, persistedMission.TargetPlanetId);
        Assert.Equal(ExplorationMissionStatus.Planned, persistedMission.Status);
        Assert.Null(persistedMission.CompletedAtUtc);
    }

    private static VoidEmpiresDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<VoidEmpiresDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new VoidEmpiresDbContext(options);
    }
}
