using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.StrategicMap;
using VoidEmpires.Domain.Exploration;
using VoidEmpires.Infrastructure.Persistence;
using VoidEmpires.Infrastructure.StrategicMap;

namespace VoidEmpires.Tests;

public class ExplorationKnowledgeQueryServiceTests
{
    [Fact]
    public async Task GetAsyncReturnsOnlyRequestingCivilizationKnowledgeInDeterministicOrder()
    {
        await using var dbContext = CreateDbContext();
        var civilizationId = Guid.NewGuid();
        var otherCivilizationId = Guid.NewGuid();
        var firstSystemId = Guid.Parse("10000000-0000-0000-0000-000000000001");
        var secondSystemId = Guid.Parse("20000000-0000-0000-0000-000000000001");
        var firstPlanetId = Guid.Parse("30000000-0000-0000-0000-000000000001");
        var discoveredAtUtc = new DateTime(2026, 6, 1, 12, 0, 0, DateTimeKind.Utc);
        dbContext.ExplorationKnowledge.AddRange(
            ExplorationKnowledge.Create(civilizationId, secondSystemId, null, ExplorationKnowledgeSource.Seeded, null, discoveredAtUtc.AddMinutes(5)),
            ExplorationKnowledge.Create(otherCivilizationId, firstSystemId, null, ExplorationKnowledgeSource.Seeded, null, discoveredAtUtc),
            ExplorationKnowledge.Create(civilizationId, firstSystemId, firstPlanetId, ExplorationKnowledgeSource.MissionCompletion, Guid.NewGuid(), discoveredAtUtc),
            ExplorationKnowledge.Create(civilizationId, firstSystemId, null, ExplorationKnowledgeSource.MissionCompletion, Guid.NewGuid(), discoveredAtUtc));
        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();

        var result = await CreateService(dbContext).GetAsync(new GetExplorationKnowledgeRequest(civilizationId));

        Assert.True(result.Succeeded);
        Assert.Equal(civilizationId, result.CivilizationId);
        Assert.Equal(3, result.Knowledge.Count);
        Assert.All(result.Knowledge, x => Assert.Equal(civilizationId, x.CivilizationId));
        Assert.Equal(
            [null, firstPlanetId, null],
            result.Knowledge.Select(x => x.PlanetId).ToArray());
        Assert.Equal(
            [firstSystemId, firstSystemId, secondSystemId],
            result.Knowledge.Select(x => x.SystemId).ToArray());
    }

    [Fact]
    public async Task GetAsyncRejectsEmptyCivilizationIdAndDoesNotMutateState()
    {
        await using var dbContext = CreateDbContext();
        var civilizationId = Guid.NewGuid();
        dbContext.ExplorationKnowledge.Add(ExplorationKnowledge.Create(
            civilizationId,
            Guid.NewGuid(),
            null,
            ExplorationKnowledgeSource.Seeded,
            null,
            new DateTime(2026, 6, 1, 12, 0, 0, DateTimeKind.Utc)));
        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();
        var countBefore = await dbContext.ExplorationKnowledge.CountAsync();

        var invalidResult = await CreateService(dbContext).GetAsync(new GetExplorationKnowledgeRequest(Guid.Empty));
        var validResult = await CreateService(dbContext).GetAsync(new GetExplorationKnowledgeRequest(civilizationId));

        Assert.False(invalidResult.Succeeded);
        Assert.Contains("Civilization id is required.", invalidResult.Errors);
        Assert.True(validResult.Succeeded);
        Assert.Equal(countBefore, await dbContext.ExplorationKnowledge.CountAsync());
        Assert.Empty(dbContext.ChangeTracker.Entries().Where(x => x.State != EntityState.Unchanged));
    }

    private static ExplorationKnowledgeQueryService CreateService(VoidEmpiresDbContext dbContext) => new(dbContext);

    private static VoidEmpiresDbContext CreateDbContext() =>
        new(new DbContextOptionsBuilder<VoidEmpiresDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);
}
