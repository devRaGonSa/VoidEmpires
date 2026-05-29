using VoidEmpires.Domain.Fleets;

namespace VoidEmpires.Application.Fleets;

public sealed record OrbitalTransferQueryRequest(
    Guid CivilizationId,
    Guid? OrbitalGroupId = null,
    Guid? OriginPlanetId = null,
    Guid? DestinationPlanetId = null,
    OrbitalTransferStatus? Status = null);
