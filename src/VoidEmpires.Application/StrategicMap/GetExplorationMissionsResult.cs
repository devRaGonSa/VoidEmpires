using VoidEmpires.Domain.Exploration;

namespace VoidEmpires.Application.StrategicMap;

public sealed record GetExplorationMissionsResult(
    Guid CivilizationId,
    ExplorationMissionStatus? Status,
    IReadOnlyList<ExplorationMissionDto> Missions,
    IReadOnlyList<string> Errors)
{
    public bool Succeeded => Errors.Count == 0;

    public static GetExplorationMissionsResult Success(
        Guid civilizationId,
        ExplorationMissionStatus? status,
        IReadOnlyList<ExplorationMissionDto> missions) =>
        new(civilizationId, status, missions, []);

    public static GetExplorationMissionsResult Invalid(
        Guid civilizationId,
        ExplorationMissionStatus? status,
        params string[] errors) =>
        new(civilizationId, status, [], errors);
}

public sealed record ExplorationMissionDto(
    Guid ExplorationMissionId,
    Guid CivilizationId,
    Guid TargetSystemId,
    Guid? TargetPlanetId,
    ExplorationMissionStatus Status,
    DateTime RequestedAtUtc,
    DateTime DueAtUtc,
    DateTime? CompletedAtUtc);
