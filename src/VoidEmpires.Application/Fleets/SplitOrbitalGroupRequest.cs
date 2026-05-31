namespace VoidEmpires.Application.Fleets;

public sealed record SplitOrbitalGroupRequest(
    Guid CivilizationId,
    Guid SourceOrbitalGroupId,
    int Quantity);

public sealed record SplitOrbitalGroupResult(
    bool Succeeded,
    Guid? SourceOrbitalGroupId,
    Guid? NewOrbitalGroupId,
    int SourceQuantity,
    int NewQuantity,
    IReadOnlyList<string> Errors)
{
    public static SplitOrbitalGroupResult Success(
        Guid sourceOrbitalGroupId,
        Guid newOrbitalGroupId,
        int sourceQuantity,
        int newQuantity)
        => new(true, sourceOrbitalGroupId, newOrbitalGroupId, sourceQuantity, newQuantity, []);

    public static SplitOrbitalGroupResult Failure(params string[] errors)
        => new(false, null, null, 0, 0, errors);
}
