namespace VoidEmpires.Application.StrategicMap;

public sealed record CreateExplorationMissionRequest(
    Guid CivilizationId,
    Guid TargetSystemId,
    Guid? TargetPlanetId,
    DateTime RequestedAtUtc);
