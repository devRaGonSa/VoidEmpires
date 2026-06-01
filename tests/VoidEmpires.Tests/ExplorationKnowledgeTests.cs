using VoidEmpires.Domain.Exploration;

namespace VoidEmpires.Tests;

public class ExplorationKnowledgeTests
{
    private static readonly DateTime DiscoveredAtUtc = new(2026, 6, 1, 12, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void CreateSupportsSystemAndPlanetKnowledge()
    {
        var planetId = Guid.NewGuid();
        var missionId = Guid.NewGuid();

        var systemKnowledge = Create();
        var planetKnowledge = Create(planetId: planetId, source: ExplorationKnowledgeSource.MissionCompletion, sourceMissionId: missionId);

        Assert.Null(systemKnowledge.PlanetId);
        Assert.Equal(planetId, planetKnowledge.PlanetId);
        Assert.Equal(missionId, planetKnowledge.SourceMissionId);
    }

    [Fact]
    public void CreateRejectsInvalidInputs()
    {
        Assert.Throws<ArgumentException>(() => Create(civilizationId: Guid.Empty));
        Assert.Throws<ArgumentException>(() => Create(systemId: Guid.Empty));
        Assert.Throws<ArgumentException>(() => Create(planetId: Guid.Empty));
        Assert.Throws<ArgumentException>(() => Create(sourceMissionId: Guid.Empty));
        Assert.Throws<ArgumentException>(() => Create(source: (ExplorationKnowledgeSource)99));
        Assert.Throws<ArgumentException>(() => Create(discoveredAtUtc: DateTime.SpecifyKind(DiscoveredAtUtc, DateTimeKind.Local)));
    }

    private static ExplorationKnowledge Create(
        Guid? civilizationId = null,
        Guid? systemId = null,
        Guid? planetId = null,
        ExplorationKnowledgeSource source = ExplorationKnowledgeSource.Seeded,
        Guid? sourceMissionId = null,
        DateTime? discoveredAtUtc = null) =>
        ExplorationKnowledge.Create(civilizationId ?? Guid.NewGuid(), systemId ?? Guid.NewGuid(), planetId, source, sourceMissionId, discoveredAtUtc ?? DiscoveredAtUtc);
}
