using VoidEmpires.Domain.Exploration;

namespace VoidEmpires.Application.StrategicMap;

public sealed record GetExplorationKnowledgeResult(
    Guid CivilizationId,
    IReadOnlyList<ExplorationKnowledgeDto> Knowledge,
    IReadOnlyList<string> Errors)
{
    public bool Succeeded => Errors.Count == 0;

    public static GetExplorationKnowledgeResult Success(
        Guid civilizationId,
        IReadOnlyList<ExplorationKnowledgeDto> knowledge) =>
        new(civilizationId, knowledge, []);

    public static GetExplorationKnowledgeResult Invalid(Guid civilizationId, params string[] errors) =>
        new(civilizationId, [], errors);
}

public sealed record ExplorationKnowledgeDto(
    Guid ExplorationKnowledgeId,
    Guid CivilizationId,
    Guid SystemId,
    Guid? PlanetId,
    ExplorationKnowledgeSource Source,
    Guid? SourceMissionId,
    DateTime DiscoveredAtUtc);
