namespace VoidEmpires.Application.Fleets;

public sealed record EstimateOrbitalTravelRequest(
    Guid CivilizationId,
    Guid OrbitalGroupId,
    Guid DestinationPlanetId);
