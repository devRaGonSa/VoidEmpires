namespace VoidEmpires.Application.Fleets;

public enum CancelOrbitalTransferResultStatus
{
    Succeeded = 1,
    ValidationFailed = 2,
    NotFound = 3,
    Conflict = 4
}

public sealed record CancelOrbitalTransferResult(
    bool Succeeded,
    CancelOrbitalTransferResultStatus Status,
    Guid? OrbitalTransferId,
    Guid? OrbitalGroupId,
    IReadOnlyList<string> Errors)
{
    public static CancelOrbitalTransferResult Success(Guid orbitalTransferId, Guid orbitalGroupId) =>
        new(true, CancelOrbitalTransferResultStatus.Succeeded, orbitalTransferId, orbitalGroupId, []);

    public static CancelOrbitalTransferResult ValidationFailure(params string[] errors) =>
        new(false, CancelOrbitalTransferResultStatus.ValidationFailed, null, null, errors);

    public static CancelOrbitalTransferResult NotFound(params string[] errors) =>
        new(false, CancelOrbitalTransferResultStatus.NotFound, null, null, errors);

    public static CancelOrbitalTransferResult Conflict(params string[] errors) =>
        new(false, CancelOrbitalTransferResultStatus.Conflict, null, null, errors);
}
