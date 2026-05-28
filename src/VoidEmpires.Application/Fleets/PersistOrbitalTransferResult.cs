namespace VoidEmpires.Application.Fleets;

public sealed record PersistOrbitalTransferResult(
    bool Succeeded,
    Guid? OrbitalTransferId,
    Guid? OrbitalGroupId,
    Guid? OriginPlanetId,
    Guid? DestinationPlanetId,
    int AbstractDistanceUnits,
    DateTime? DepartureAtUtc,
    DateTime? ArrivalAtUtc,
    IReadOnlyList<string> Errors)
{
    public static PersistOrbitalTransferResult Success(
        Guid orbitalTransferId,
        Guid orbitalGroupId,
        Guid originPlanetId,
        Guid destinationPlanetId,
        int abstractDistanceUnits,
        DateTime departureAtUtc,
        DateTime arrivalAtUtc) =>
        new(
            true,
            orbitalTransferId,
            orbitalGroupId,
            originPlanetId,
            destinationPlanetId,
            abstractDistanceUnits,
            departureAtUtc,
            arrivalAtUtc,
            []);

    public static PersistOrbitalTransferResult Failure(params string[] errors) =>
        new(false, null, null, null, null, 0, null, null, errors);
}
