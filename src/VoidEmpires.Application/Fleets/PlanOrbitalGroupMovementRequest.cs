namespace VoidEmpires.Application.Fleets;

public sealed record PlanOrbitalGroupMovementRequest(
    Guid CivilizationId,
    Guid OrbitalGroupId,
    Guid DestinationPlanetId,
    DateTime RequestedAtUtc);
