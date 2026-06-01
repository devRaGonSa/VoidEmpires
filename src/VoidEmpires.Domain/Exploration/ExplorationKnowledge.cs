namespace VoidEmpires.Domain.Exploration;

public sealed class ExplorationKnowledge
{
    private ExplorationKnowledge() { }

    private ExplorationKnowledge(
        Guid civilizationId,
        Guid systemId,
        Guid? planetId,
        ExplorationKnowledgeSource source,
        Guid? sourceMissionId,
        DateTime discoveredAtUtc)
    {
        if (civilizationId == Guid.Empty) throw new ArgumentException("Civilization id is required.", nameof(civilizationId));
        if (systemId == Guid.Empty) throw new ArgumentException("System id is required.", nameof(systemId));
        if (planetId == Guid.Empty) throw new ArgumentException("Planet id cannot be empty.", nameof(planetId));
        if (!Enum.IsDefined(source)) throw new ArgumentException("Exploration knowledge source is invalid.", nameof(source));
        if (sourceMissionId == Guid.Empty) throw new ArgumentException("Source mission id cannot be empty.", nameof(sourceMissionId));
        if (discoveredAtUtc.Kind != DateTimeKind.Utc) throw new ArgumentException("Discovery date must be UTC.", nameof(discoveredAtUtc));

        Id = Guid.NewGuid();
        CivilizationId = civilizationId;
        SystemId = systemId;
        PlanetId = planetId;
        Source = source;
        SourceMissionId = sourceMissionId;
        DiscoveredAtUtc = discoveredAtUtc;
    }

    public Guid Id { get; private set; }
    public Guid CivilizationId { get; private set; }
    public Guid SystemId { get; private set; }
    public Guid? PlanetId { get; private set; }
    public ExplorationKnowledgeSource Source { get; private set; }
    public Guid? SourceMissionId { get; private set; }
    public DateTime DiscoveredAtUtc { get; private set; }

    public static ExplorationKnowledge Create(
        Guid civilizationId,
        Guid systemId,
        Guid? planetId,
        ExplorationKnowledgeSource source,
        Guid? sourceMissionId,
        DateTime discoveredAtUtc) =>
        new(civilizationId, systemId, planetId, source, sourceMissionId, discoveredAtUtc);
}
