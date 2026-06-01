using Microsoft.EntityFrameworkCore;
using VoidEmpires.Domain.Exploration;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Tests;

public class ExplorationKnowledgePersistenceTests
{
    [Fact]
    public async Task DbContextPersistsKnowledgeAndDefinesDuplicatePreventionIndexes()
    {
        await using var dbContext = CreateDbContext();
        var knowledge = ExplorationKnowledge.Create(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), ExplorationKnowledgeSource.MissionCompletion,
            Guid.NewGuid(), new DateTime(2026, 6, 1, 12, 0, 0, DateTimeKind.Utc));

        dbContext.ExplorationKnowledge.Add(knowledge);
        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();

        var persisted = await dbContext.ExplorationKnowledge.SingleAsync();
        var indexes = dbContext.Model.FindEntityType(typeof(ExplorationKnowledge))!.GetIndexes();

        Assert.Equal((knowledge.Id, knowledge.PlanetId), (persisted.Id, persisted.PlanetId));
        Assert.Contains(indexes, x => IsIndex(x, "\"PlanetId\" IS NULL", "CivilizationId", "SystemId"));
        Assert.Contains(indexes, x => IsIndex(x, "\"PlanetId\" IS NOT NULL", "CivilizationId", "SystemId", "PlanetId"));
    }

    private static bool IsIndex(Microsoft.EntityFrameworkCore.Metadata.IIndex index, string filter, params string[] names) =>
        index.IsUnique && index.GetFilter() == filter && index.Properties.Select(x => x.Name).SequenceEqual(names);

    private static VoidEmpiresDbContext CreateDbContext() =>
        new(new DbContextOptionsBuilder<VoidEmpiresDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);
}
