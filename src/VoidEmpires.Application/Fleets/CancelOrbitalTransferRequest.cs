namespace VoidEmpires.Application.Fleets;

public sealed record CancelOrbitalTransferRequest(
    Guid CivilizationId,
    Guid OrbitalTransferId);
