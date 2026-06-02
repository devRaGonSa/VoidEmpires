namespace VoidEmpires.Application.Fleets;

public enum PersistOrbitalTransferResultStatus
{
    Succeeded = 0,
    ValidationFailed = 1,
    NotFound = 2,
    Conflict = 3
}

public sealed record PersistOrbitalTransferResult(
    PersistOrbitalTransferResultStatus Status,
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
            PersistOrbitalTransferResultStatus.Succeeded,
            true,
            orbitalTransferId,
            orbitalGroupId,
            originPlanetId,
            destinationPlanetId,
            abstractDistanceUnits,
            departureAtUtc,
            arrivalAtUtc,
            []);

    public static PersistOrbitalTransferResult ValidationFailure(params string[] errors) =>
        new(PersistOrbitalTransferResultStatus.ValidationFailed, false, null, null, null, null, 0, null, null, errors);

    public static PersistOrbitalTransferResult NotFound(params string[] errors) =>
        new(PersistOrbitalTransferResultStatus.NotFound, false, null, null, null, null, 0, null, null, errors);

    public static PersistOrbitalTransferResult Conflict(params string[] errors) =>
        new(PersistOrbitalTransferResultStatus.Conflict, false, null, null, null, null, 0, null, null, errors);

    public static PersistOrbitalTransferResult Failure(params string[] errors) =>
        Conflict(errors);
}
