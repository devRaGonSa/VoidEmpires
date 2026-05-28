namespace VoidEmpires.Application.Fleets;

public sealed record PersistOrbitalTransferRequest(
    Guid CivilizationId,
    Guid OrbitalGroupId,
    Guid DestinationPlanetId,
    DateTime RequestedAtUtc);
