namespace VoidEmpires.Application.Fleets;

public sealed record PlanOrbitalGroupTransferRequest(
    Guid CivilizationId,
    Guid OrbitalGroupId,
    Guid DestinationPlanetId,
    DateTime RequestedAtUtc);
