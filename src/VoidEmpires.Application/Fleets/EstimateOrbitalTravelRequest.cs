using VoidEmpires.Domain.Economy;

namespace VoidEmpires.Application.Fleets;

public sealed record EstimateOrbitalTravelRequest(
    Guid CivilizationId,
    Guid OrbitalGroupId,
    Guid DestinationPlanetId);

public sealed record EstimateOrbitalTravelResult(
    bool Succeeded,
    Guid? OrbitalGroupId,
    Guid? CurrentPlanetId,
    Guid? DestinationPlanetId,
    int AbstractDistanceUnits,
    TimeSpan? EstimatedDuration,
    IReadOnlyList<OrbitalTravelCostComponentDto> EstimatedCosts,
    IReadOnlyList<string> Errors)
{
    public static EstimateOrbitalTravelResult Success(
        Guid orbitalGroupId,
        Guid currentPlanetId,
        Guid destinationPlanetId,
        int abstractDistanceUnits,
        TimeSpan estimatedDuration,
        IReadOnlyList<OrbitalTravelCostComponentDto> estimatedCosts) =>
        new(true, orbitalGroupId, currentPlanetId, destinationPlanetId, abstractDistanceUnits, estimatedDuration, estimatedCosts, []);

    public static EstimateOrbitalTravelResult Failure(params string[] errors) =>
        new(false, null, null, null, 0, null, [], errors);
}

public sealed record OrbitalTravelCostComponentDto(ResourceType ResourceType, int Amount);
