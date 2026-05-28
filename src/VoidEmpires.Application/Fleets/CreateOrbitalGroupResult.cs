namespace VoidEmpires.Application.Fleets;

public sealed record CreateOrbitalGroupResult(
    bool Succeeded,
    Guid? OrbitalGroupId,
    IReadOnlyList<string> Errors)
{
    public static CreateOrbitalGroupResult Success(Guid orbitalGroupId)
        => new(true, orbitalGroupId, []);

    public static CreateOrbitalGroupResult Failure(params string[] errors)
        => new(false, null, errors);
}
