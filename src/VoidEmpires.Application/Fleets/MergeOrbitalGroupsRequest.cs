namespace VoidEmpires.Application.Fleets;

public sealed record MergeOrbitalGroupsRequest(
    Guid CivilizationId,
    Guid TargetOrbitalGroupId,
    Guid SourceOrbitalGroupId);

public sealed record MergeOrbitalGroupsResult(
    bool Succeeded,
    Guid? TargetOrbitalGroupId,
    Guid? SourceOrbitalGroupId,
    int TargetQuantity,
    IReadOnlyList<string> Errors)
{
    public static MergeOrbitalGroupsResult Success(
        Guid targetOrbitalGroupId,
        Guid sourceOrbitalGroupId,
        int targetQuantity)
        => new(true, targetOrbitalGroupId, sourceOrbitalGroupId, targetQuantity, []);

    public static MergeOrbitalGroupsResult Failure(params string[] errors)
        => new(false, null, null, 0, errors);
}
