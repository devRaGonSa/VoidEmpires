namespace VoidEmpires.Application.Fleets;

public enum EstimateOrbitalTravelResultStatus
{
    Succeeded = 0,
    ValidationFailed = 1,
    NotFound = 2,
    Conflict = 3
}

public sealed record EstimateOrbitalTravelResult(
    EstimateOrbitalTravelResultStatus Status,
    bool Succeeded,
    Guid? OrbitalGroupId,
    Guid? CurrentPlanetId,
    Guid? DestinationPlanetId,
    int AbstractDistanceUnits,
    TimeSpan? EstimatedDuration,
    OrbitalRouteProfileDto? RouteProfile,
    OrbitalFuelReadinessDto? FuelReadiness,
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
        OrbitalRouteProfileDto routeProfile,
        OrbitalFuelReadinessDto fuelReadiness,
        IReadOnlyList<OrbitalTravelCostComponentDto> resourceCosts,
        bool canAfford,
        IReadOnlyList<OrbitalTravelInsufficientResourceDto> insufficientResources) =>
        new(
            EstimateOrbitalTravelResultStatus.Succeeded,
            true,
            orbitalGroupId,
            currentPlanetId,
            destinationPlanetId,
            abstractDistanceUnits,
            estimatedDuration,
            routeProfile,
            fuelReadiness,
            resourceCosts,
            canAfford,
            insufficientResources,
            []);

    public static EstimateOrbitalTravelResult ValidationFailure(params string[] errors) =>
        new(EstimateOrbitalTravelResultStatus.ValidationFailed, false, null, null, null, 0, null, null, null, [], false, [], errors);

    public static EstimateOrbitalTravelResult NotFound(params string[] errors) =>
        new(EstimateOrbitalTravelResultStatus.NotFound, false, null, null, null, 0, null, null, null, [], false, [], errors);

    public static EstimateOrbitalTravelResult Conflict(params string[] errors) =>
        new(EstimateOrbitalTravelResultStatus.Conflict, false, null, null, null, 0, null, null, null, [], false, [], errors);

    public static EstimateOrbitalTravelResult Failure(params string[] errors) =>
        Conflict(errors);
}
