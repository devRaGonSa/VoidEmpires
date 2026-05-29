using VoidEmpires.Domain.Fleets;

namespace VoidEmpires.Application.Fleets;

public sealed record OrbitalTransferQueryItem(
    Guid Id,
    Guid CivilizationId,
    Guid OrbitalGroupId,
    Guid OriginPlanetId,
    Guid DestinationPlanetId,
    int AbstractDistanceUnits,
    DateTime DepartureAtUtc,
    DateTime ArrivalAtUtc,
    OrbitalTransferStatus Status);
