namespace VoidEmpires.Application.Fleets;

public sealed record EstimateOrbitalTravelResult(
    bool Succeeded,
    Guid? OrbitalGroupId,
    Guid? CurrentPlanetId,
    Guid? DestinationPlanetId,
    int AbstractDistanceUnits,
    TimeSpan? EstimatedDuration,
    IReadOnlyList<OrbitalTravelCostComponentDto> ResourceCosts,
    bool CanAfford,
    IReadOnlyList<OrbitalTravelInsufficientResourceDto> InsufficientResources,
    IReadOnlyList<string> Errors)
{
    public static EstimateOrbitalTravelResult Success(
        Guid orbitalGroupId,
        Guid currentPlanetId,
        Guid destinationPlanetId,
        int abstractDistanceUnits,
        TimeSpan estimatedDuration,
        IReadOnlyList<OrbitalTravelCostComponentDto> resourceCosts,
        bool canAfford,
        IReadOnlyList<OrbitalTravelInsufficientResourceDto> insufficientResources) =>
        new(
            true,
            orbitalGroupId,
            currentPlanetId,
            destinationPlanetId,
            abstractDistanceUnits,
            estimatedDuration,
            resourceCosts,
            canAfford,
            insufficientResources,
            []);

    public static EstimateOrbitalTravelResult Failure(params string[] errors) =>
        new(false, null, null, null, 0, null, [], false, [], errors);
}
