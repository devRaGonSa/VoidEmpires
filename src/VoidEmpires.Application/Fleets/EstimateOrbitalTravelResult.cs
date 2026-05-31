namespace VoidEmpires.Application.Fleets;

public sealed record EstimateOrbitalTravelResult(
    bool Succeeded,
    Guid? OrbitalGroupId,
    Guid? CurrentPlanetId,
    Guid? DestinationPlanetId,
    int AbstractDistanceUnits,
    TimeSpan? EstimatedDuration,
    IReadOnlyList<OrbitalTravelCostComponentDto> ResourceCosts,
    IReadOnlyList<string> Errors)
{
    public static EstimateOrbitalTravelResult Success(
        Guid orbitalGroupId,
        Guid currentPlanetId,
        Guid destinationPlanetId,
        int abstractDistanceUnits,
        TimeSpan estimatedDuration,
        IReadOnlyList<OrbitalTravelCostComponentDto> resourceCosts) =>
        new(
            true,
            orbitalGroupId,
            currentPlanetId,
            destinationPlanetId,
            abstractDistanceUnits,
            estimatedDuration,
            resourceCosts,
            []);

    public static EstimateOrbitalTravelResult Failure(params string[] errors) =>
        new(false, null, null, null, 0, null, [], errors);
}
