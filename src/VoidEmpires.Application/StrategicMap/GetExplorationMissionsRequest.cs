using VoidEmpires.Domain.Exploration;

namespace VoidEmpires.Application.StrategicMap;

public sealed record GetExplorationMissionsRequest(
    Guid CivilizationId,
    ExplorationMissionStatus? Status);
