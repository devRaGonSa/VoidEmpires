namespace VoidEmpires.Application.Fleets;

public sealed record PlanOrbitalGroupTransferResult(
    bool Succeeded,
    Guid? OrbitalGroupId,
    Guid? CurrentPlanetId,
    Guid? DestinationPlanetId,
    int AbstractDistanceUnits,
    DateTime? DepartureAtUtc,
    DateTime? ArrivalAtUtc,
    IReadOnlyList<string> Errors)
{
    public static PlanOrbitalGroupTransferResult Success(
        Guid orbitalGroupId,
        Guid currentPlanetId,
        Guid destinationPlanetId,
        int abstractDistanceUnits,
        DateTime departureAtUtc,
        DateTime arrivalAtUtc) =>
        new(
            true,
            orbitalGroupId,
            currentPlanetId,
            destinationPlanetId,
            abstractDistanceUnits,
            departureAtUtc,
            arrivalAtUtc,
            []);

    public static PlanOrbitalGroupTransferResult Failure(params string[] errors) =>
        new(false, null, null, null, 0, null, null, errors);
}
