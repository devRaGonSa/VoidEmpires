using VoidEmpires.Domain.Exploration;

namespace VoidEmpires.Application.StrategicMap;

public sealed record CreateExplorationMissionResult(
    bool Succeeded,
    CreatedExplorationMissionDto? Mission,
    IReadOnlyList<string> Errors,
    bool IsConflict)
{
    public static CreateExplorationMissionResult Success(CreatedExplorationMissionDto mission) =>
        new(true, mission, [], false);

    public static CreateExplorationMissionResult Invalid(params string[] errors) =>
        new(false, null, errors, false);

    public static CreateExplorationMissionResult Conflict(params string[] errors) =>
        new(false, null, errors, true);
}

public sealed record CreatedExplorationMissionDto(
    Guid ExplorationMissionId,
    Guid CivilizationId,
    Guid TargetSystemId,
    Guid? TargetPlanetId,
    ExplorationMissionStatus Status,
    DateTime RequestedAtUtc,
    DateTime DueAtUtc);
